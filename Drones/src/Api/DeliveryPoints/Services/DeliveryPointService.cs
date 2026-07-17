using Drones.src.Api.Data;
using Drones.src.Api.DeliveryPoints.DTOs.Requests;
using Drones.src.Api.DeliveryPoints.DTOs.Responses;
using Drones.src.Api.DeliveryPoints.Entities;
using Microsoft.EntityFrameworkCore;

namespace Drones.src.Api.DeliveryPoints.Services
{
    public class DeliveryPointService : IDeliveryPointService
    {
        private readonly AppDbContext _context;

        public DeliveryPointService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<DeliveryPointResponse>> GetActiveAsync()
        {
            var points = await _context.DeliveryPoints
                .Where(p => p.IsActive)
                .OrderBy(p => p.Name)
                .ToListAsync();

            return points.Select(MapToResponse).ToList();
        }

        public async Task<List<DeliveryPointResponse>> GetAllAsync()
        {
            var points = await _context.DeliveryPoints
                .OrderBy(p => p.Name)
                .ToListAsync();

            return points.Select(MapToResponse).ToList();
        }

        public async Task<DeliveryPointResponse> GetAsync(Guid id)
        {
            var point = await _context.DeliveryPoints.FirstOrDefaultAsync(p => p.Id == id)
                ?? throw new InvalidOperationException("DELIVERY_POINT_NOT_FOUND");

            return MapToResponse(point);
        }

        public async Task<DeliveryPointResponse> CreateAsync(CreateDeliveryPointRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new InvalidOperationException("Delivery point name is required.");

            var point = new DeliveryPoint
            {
                Id = Guid.NewGuid(),
                Name = request.Name.Trim(),
                Address = request.Address.Trim(),
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                IsActive = request.IsActive,
                CreatedAt = DateTime.UtcNow,
            };

            await _context.DeliveryPoints.AddAsync(point);
            await _context.SaveChangesAsync();

            return MapToResponse(point);
        }

        public async Task<DeliveryPointResponse> UpdateAsync(Guid id, UpdateDeliveryPointRequest request)
        {
            var point = await _context.DeliveryPoints.FirstOrDefaultAsync(p => p.Id == id)
                ?? throw new InvalidOperationException("DELIVERY_POINT_NOT_FOUND");

            if (request.Name != null) point.Name = request.Name.Trim();
            if (request.Address != null) point.Address = request.Address.Trim();
            if (request.Latitude.HasValue) point.Latitude = request.Latitude.Value;
            if (request.Longitude.HasValue) point.Longitude = request.Longitude.Value;
            if (request.IsActive.HasValue) point.IsActive = request.IsActive.Value;
            point.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return MapToResponse(point);
        }

        public async Task DeactivateAsync(Guid id)
        {
            var point = await _context.DeliveryPoints.FirstOrDefaultAsync(p => p.Id == id)
                ?? throw new InvalidOperationException("DELIVERY_POINT_NOT_FOUND");

            point.IsActive = false;
            point.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        private static DeliveryPointResponse MapToResponse(DeliveryPoint point) => new()
        {
            Id = point.Id,
            Name = point.Name,
            Address = point.Address,
            Latitude = point.Latitude,
            Longitude = point.Longitude,
            IsActive = point.IsActive,
            CreatedAt = point.CreatedAt,
            UpdatedAt = point.UpdatedAt,
        };
    }
}
