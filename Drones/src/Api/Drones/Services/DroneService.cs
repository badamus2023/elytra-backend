using Drones.src.Api.Data;
using Drones.src.Api.Drones.DTOs.Requests;
using Drones.src.Api.Drones.DTOs.Responses;
using Drones.src.Api.Drones.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.Json;
using System.Runtime.InteropServices;

namespace Drones.src.Api.Drones.Services
{
    public class DroneService : IDroneService
    {
        private readonly AppDbContext _context;

        public DroneService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<DroneResponse> CreateDroneAsync(CreateDroneRequest request)
        {
            var drone = new Drone
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                MaxPayloadKg = request.MaxPayloadKg,
                CurrentLatitude = request.HomeLatitude,
                CurrentLongitude = request.HomeLongitude,
                BatteryLevel = 100,
                Status = DroneStatus.Idle,
                CreatedAt = DateTime.UtcNow,
                LastSeenAt = DateTime.UtcNow,
            };

            await _context.Drones.AddAsync(drone);
            await _context.SaveChangesAsync();

            return MapToResponse(drone);
        }

        public async Task DeleteDroneAsync(Guid droneId)
        {
            var drone = await _context.Drones.FirstOrDefaultAsync(d => d.Id == droneId)
                 ?? throw new InvalidOperationException("DRONE_NOT_FOUND");

            if (drone.Status == DroneStatus.InFlight)
                throw new InvalidOperationException("Cannot delete a drone that is in flight.");

            _context.Drones.Remove(drone);
            await _context.SaveChangesAsync();
        }

        public async Task<List<DroneResponse>> GetAllDronesAsync()
        {
            var drones = await _context.Drones.OrderBy(d => d.Name).ToListAsync();

            return drones.Select(MapToResponse).ToList();
        }

        public async Task<List<DroneResponse>> GetAvailableDronesAsync()
        {
            var drones = await _context.Drones.Where(d => d.Status == DroneStatus.Idle && d.BatteryLevel >= 20)
                .OrderByDescending(d => d.BatteryLevel)
                .ToListAsync();

            return drones.Select(MapToResponse).ToList();
        }

        public async Task<DroneResponse> GetDroneAsync(Guid droneId)
        {
            var drone = await _context.Drones.FirstOrDefaultAsync(d => d.Id == droneId)
                ?? throw new InvalidOperationException("DRONE_NOT_FOUND");

            return MapToResponse(drone);
        }

        public async Task<DroneResponse> UpdateDroneAsync(Guid droneId, UpdateDroneRequest request)
        {
            var drone = await _context.Drones.FirstOrDefaultAsync(d => d.Id == droneId)
                ?? throw new InvalidOperationException("DRONE_NOT_FOUND");

            if(request.Name != null)
                drone.Name = request.Name;

            if (request.MaxPayloadKg != null)
                drone.MaxPayloadKg = request.MaxPayloadKg.Value;

            if(request.BatteryLevel != null)
                drone.BatteryLevel = request.BatteryLevel.Value;

            await _context.SaveChangesAsync();

            return MapToResponse(drone);
        }

        public Task UpdateDroneLocationAsync(Guid droneId, double latitude, double longitude)
        {
            throw new NotImplementedException();
        }

        //helpers
        private DroneResponse MapToResponse(Drone drone) => new()
        {
            Id = drone.Id,
            Name = drone.Name,
            Status = drone.Status.ToString(),
            BatteryLevel = drone.BatteryLevel,
            MaxPayloadKg = drone.MaxPayloadKg,
            CurrentLatitude = drone.CurrentLatitude,
            CurrentLongitude = drone.CurrentLongitude,
            LastSeenAt = drone.LastSeenAt,
            CreatedAt = drone.CreatedAt,
        };
    }
}
