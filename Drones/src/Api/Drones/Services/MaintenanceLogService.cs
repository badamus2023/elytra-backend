using Drones.src.Api.Data;
using Drones.src.Api.Drones.DTOs.Requests;
using Drones.src.Api.Drones.DTOs.Responses;
using Drones.src.Api.Drones.Entities;
using Microsoft.EntityFrameworkCore;

namespace Drones.src.Api.Drones.Services
{
    public class MaintenanceLogService : IMaintenanceLogService
    {
        private readonly AppDbContext _context;

        public MaintenanceLogService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<MaintenanceLogResponse> CreateAsync(CreateMaintenanceLogRequest request)
        {
            var drone = await _context.Drones.FirstOrDefaultAsync(d => d.Id == request.DroneId)
                ?? throw new InvalidOperationException("DRONE_NOT_FOUND");

            var log = new DroneMaintenanceLog
            {
                Id = Guid.NewGuid(),
                DroneId = drone.Id,
                Type = Enum.Parse<MaintenanceType>(request.Type, ignoreCase: true),
                Notes = request.Notes,
                PerformedAt = request.PerformedAt,
                CreatedAt = DateTime.UtcNow,
            };

            drone.Status = DroneStatus.Maintenance;

            await _context.DroneMaintenanceLogs.AddAsync(log);
            await _context.SaveChangesAsync();

            return MapToResponse(log);
        }

        public async Task DeleteAsync(Guid id)
        {
            var log = await _context.DroneMaintenanceLogs.FirstOrDefaultAsync(l => l.Id == id)
                ?? throw new InvalidOperationException("LOG_NOT_FOUND");

            _context.DroneMaintenanceLogs.Remove(log);
            await _context.SaveChangesAsync();
        }

        public async Task<MaintenanceLogResponse> GetAsync(Guid id)
        {
            var log = await _context.DroneMaintenanceLogs.FirstOrDefaultAsync(l => l.Id == id)
                ?? throw new InvalidOperationException("LOG_NOT_FOUND");

            return MapToResponse(log);
        }

        public async Task<List<MaintenanceLogResponse>> GetByDroneAsync(Guid droneId)
        {
            var logs = await _context.DroneMaintenanceLogs.Where(l => l.DroneId == droneId)
                .OrderByDescending(l => l.PerformedAt)
                .ToListAsync();

            return logs.Select(MapToResponse).ToList();
        }

        public async Task<MaintenanceLogResponse> UpdateAsync(Guid id, UpdateMaintenanceLogRequest request)
        {
            var log = await _context.DroneMaintenanceLogs
                .FirstOrDefaultAsync(l => l.Id == id)
                ?? throw new InvalidOperationException("LOG_NOT_FOUND");

            if(request.Notes != null)
            {
                log.Notes = request.Notes;
            }

            if (request.PerformedAt != null)
            {
                log.PerformedAt = request.PerformedAt.Value;
            }

            log.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return MapToResponse(log);

        }

        //helpers
        private static MaintenanceLogResponse MapToResponse(DroneMaintenanceLog log) => new()
        {
            Id = log.Id,
            DroneId = log.DroneId,
            Type = log.Type.ToString(),
            Notes = log.Notes,
            PerformedAt = log.PerformedAt,
            CreatedAt = log.CreatedAt
        };
    }
}
