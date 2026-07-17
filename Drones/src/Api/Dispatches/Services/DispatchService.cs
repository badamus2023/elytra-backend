using Drones.src.Api.Data;
using Drones.src.Api.Dispatches.DTOs.Requests;
using Drones.src.Api.Dispatches.DTOs.Responses;
using Drones.src.Api.Dispatches.Entities;
using Drones.src.Api.Drones.Entities;
using Drones.src.Api.Orders.Entities;
using Microsoft.EntityFrameworkCore;

namespace Drones.src.Api.Dispatches.Services
{
    public class DispatchService : IDispatchService
    {
        private readonly AppDbContext _context;
        private readonly IServiceScopeFactory _scopeFactory;

        public DispatchService(AppDbContext context, IServiceScopeFactory scopeFactory)
        {
            _context = context;
            _scopeFactory = scopeFactory;
        }

        public async Task<DispatchResponse> CreateDispatchAsync(CreateDispatchRequest request)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == request.OrderId)
                ?? throw new InvalidOperationException("ORDER_NOT_FOUND");

            //if (order.Status != OrderStatus.Paid)
            //    throw new InvalidOperationException("Order must be paid before dispatch");

            var existingDispatch = await _context.Dispatches.AnyAsync(d => d.OrderId == request.OrderId);

            if (existingDispatch)
                throw new InvalidOperationException("Order already has a dispatch");

            Drone drone;

            if(request.DroneId.HasValue)
            {
                drone = await _context.Drones.FirstOrDefaultAsync(d => d.Id == request.DroneId && d.Status == DroneStatus.Idle)
                    ?? throw new InvalidOperationException("Drone not found or not available");
            }
            else
            {
                drone = await _context.Drones.Where(d => d.Status == DroneStatus.Idle && d.BatteryLevel >= 20)
                   .OrderByDescending(d => d.BatteryLevel)
                   .FirstOrDefaultAsync()
                   ?? throw new InvalidOperationException("No available drones.");
            }

            var distanceKm = CalculateDistance(
                drone.CurrentLatitude, drone.CurrentLongitude,
                order.DeliveryLatitude, order.DeliveryLongitude
            );

            var estimatedMinutes = distanceKm / 0.5;
            var estimatedDeliveryAt = DateTime.UtcNow.AddMinutes(estimatedMinutes);

            var dispatch = new Dispatch
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                DroneId = drone.Id,
                Status = DispatchStatus.Assigned,
                EstimatedDeliveryAt = estimatedDeliveryAt,
                CreatedAt = DateTime.UtcNow
            };

            drone.Status = DroneStatus.Assigned;
            order.Status = OrderStatus.Dispatched;
            order.UpdatedAt = DateTime.UtcNow;

            await _context.Dispatches.AddAsync(dispatch);
            await _context.SaveChangesAsync();

            return MapToResponse(dispatch, dispatch.Drone);
        }

        public async Task<DispatchResponse> GetDispatchAsync(Guid dispatchId)
        {
            var dispatch = await _context.Dispatches.Include(d => d.Drone)
                .FirstOrDefaultAsync(d => d.Id == dispatchId)
                ?? throw new InvalidOperationException("DISPATCH_NOT_FOUND");

            return MapToResponse(dispatch, dispatch.Drone);
        }

        public async Task<DispatchResponse> GetDispatchByOrderAsync(Guid orderId)
        {
            var dispatch = await _context.Dispatches.Include(d => d.Drone)
                .FirstOrDefaultAsync(d => d.OrderId == orderId)
                ?? throw new InvalidOperationException("DISPATCH_NOT_FOUND");

            return MapToResponse(dispatch, dispatch.Drone);
        }

        public async Task SimulateFlightAsync(Guid dispatchId)
        {
            var dispatch = await _context.Dispatches
                .Include(d => d.Drone)
                .Include(d => d.Order)
                .FirstOrDefaultAsync(d => d.Id == dispatchId)
                ?? throw new InvalidOperationException("DISPATCH_NOT_FOUND");

            if (dispatch.Status != DispatchStatus.Assigned)
                throw new InvalidOperationException("Dispatch must be assigned to start simulation.");

            var startLat = dispatch.Drone.CurrentLatitude;
            var startLon = dispatch.Drone.CurrentLongitude;
            var endLat = dispatch.Order.DeliveryLatitude;
            var endLon = dispatch.Order.DeliveryLongitude;
            var droneId = dispatch.DroneId;
            var orderId = dispatch.OrderId;

            dispatch.Status = DispatchStatus.InFlight;
            dispatch.Drone.Status = DroneStatus.InFlight;
            dispatch.Order.Status = OrderStatus.InFlight;
            dispatch.Order.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            _ = Task.Run(async () =>
            {
                var steps = 10;

                for (int i = 1; i <= steps; i++)
                {
                    await Task.Delay(3000);

                    using var scope = _scopeFactory.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    var drone = await db.Drones
                        .FirstOrDefaultAsync(d => d.Id == droneId);

                    if (drone == null) break;

                    drone.CurrentLatitude = startLat + (endLat - startLat) * i / steps;
                    drone.CurrentLongitude = startLon + (endLon - startLon) * i / steps;
                    drone.LastSeenAt = DateTime.UtcNow;

                    await db.SaveChangesAsync();
                }

                using var finalScope = _scopeFactory.CreateScope();
                var finalDb = finalScope.ServiceProvider.GetRequiredService<AppDbContext>();

                var finalDispatch = await finalDb.Dispatches
                    .Include(d => d.Drone)
                    .Include(d => d.Order)
                    .FirstOrDefaultAsync(d => d.Id == dispatchId);

                if (finalDispatch == null) return;

                finalDispatch.Status = DispatchStatus.Delivered;
                finalDispatch.DeliveredAt = DateTime.UtcNow;
                finalDispatch.UpdatedAt = DateTime.UtcNow;
                finalDispatch.Drone.Status = DroneStatus.Idle;
                finalDispatch.Drone.BatteryLevel = Math.Max(0, finalDispatch.Drone.BatteryLevel - 20);
                finalDispatch.Order.Status = OrderStatus.Delivered;
                finalDispatch.Order.UpdatedAt = DateTime.UtcNow;

                await finalDb.SaveChangesAsync();
            });
        }

        public async Task<DispatchResponse> UpdateDispatchStatusAsync(Guid dispatchId, string status)
        {
            var dispatch = await _context.Dispatches
            .Include(d => d.Drone)
            .Include(d => d.Order)
            .FirstOrDefaultAsync(d => d.Id == dispatchId)
            ?? throw new InvalidOperationException("DISPATCH_NOT_FOUND");

            var newStatus = Enum.Parse<DispatchStatus>(status, ignoreCase: true);
            dispatch.Status = newStatus;
            dispatch.UpdatedAt = DateTime.UtcNow;

            switch (newStatus)
            {
                case DispatchStatus.InFlight:
                    dispatch.Drone.Status = DroneStatus.InFlight;
                    dispatch.Order.Status = OrderStatus.InFlight;
                    dispatch.Order.UpdatedAt = DateTime.UtcNow;
                    break;

                case DispatchStatus.Delivered:
                    dispatch.Drone.Status = DroneStatus.Idle;
                    dispatch.Drone.BatteryLevel = Math.Max(0, dispatch.Drone.BatteryLevel - 20);
                    dispatch.Order.Status = OrderStatus.Delivered;
                    dispatch.Order.UpdatedAt = DateTime.UtcNow;
                    dispatch.DeliveredAt = DateTime.UtcNow;
                    break;

                case DispatchStatus.Failed:
                    dispatch.Drone.Status = DroneStatus.Idle;
                    dispatch.Order.Status = OrderStatus.Cancelled;
                    dispatch.Order.UpdatedAt = DateTime.UtcNow;
                    break;
            }

            await _context.SaveChangesAsync();

            return MapToResponse(dispatch, dispatch.Drone);
        }

        //helpers

       private static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371;
            var dLat = ToRad(lat2 - lat1);
            var dLon = ToRad(lon2 - lon1);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRad(lat1)) * Math.Cos(ToRad(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        private static double ToRad(double deg) => deg * Math.PI / 180;

        private static DispatchResponse MapToResponse(Dispatch dispatch, Drone drone) => new()
        {
            Id = dispatch.Id,
            OrderId = dispatch.OrderId,
            DroneId = dispatch.DroneId,
            Status = dispatch.Status.ToString(),
            EstimatedDeliveryAt = dispatch.EstimatedDeliveryAt,
            DeliveredAt = dispatch.DeliveredAt,
            CreatedAt = dispatch.CreatedAt,
            UpdatedAt = dispatch.UpdatedAt,
            DroneLatitude = drone.CurrentLatitude,
            DroneLongitude = drone.CurrentLongitude
        };
    }
}
