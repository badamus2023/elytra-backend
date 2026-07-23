using Drones.src.Api.Common.Hubs;
using Drones.src.Api.Data;
using Drones.src.Api.Dispatches.Entities;
using Drones.src.Api.Drones.Entities;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using System.Text.Json;

namespace Drones.src.Api.Common.BackgroundServices
{
    public class DroneTelemetrySubscriber : BackgroundService
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IHubContext<DroneHub> _hubContext;
        private readonly ILogger<DroneTelemetrySubscriber> _logger;

        public DroneTelemetrySubscriber(
            IConnectionMultiplexer redis,
            IServiceScopeFactory scopeFactory,
            IHubContext<DroneHub> hubContext,
            ILogger<DroneTelemetrySubscriber> logger)
        {
            _redis = redis;
            _scopeFactory = scopeFactory;
            _hubContext = hubContext;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var sub = _redis.GetSubscriber();

            await sub.SubscribeAsync(
     RedisChannel.Pattern("drone:*:telemetry"),
     async (channel, message) =>
     {
         try
         {
             _logger.LogDebug("Received message on channel: {Channel}", channel);
             _logger.LogDebug("Raw message: {Message}", message);

             var parts = channel.ToString().Split(':');
             var droneIdStr = parts[1];
             _logger.LogDebug("Parsed droneId string: {DroneIdStr}", droneIdStr);

             Guid droneId;
             if (!Guid.TryParse(droneIdStr, out droneId))
             {
                 _logger.LogDebug("DroneId is not a Guid, looking up by index: {DroneIdStr}", droneIdStr);

                 using var lookupScope = _scopeFactory.CreateScope();
                 var lookupDb = lookupScope.ServiceProvider
                     .GetRequiredService<AppDbContext>();

                 var foundDrone = await lookupDb.Drones
                     .OrderBy(d => d.CreatedAt)
                     .Skip(int.Parse(droneIdStr) - 1)
                     .FirstOrDefaultAsync();

                 if (foundDrone == null)
                 {
                     _logger.LogWarning("No drone found for index: {DroneIdStr} — will skip DB update", droneIdStr);
                     droneId = Guid.Empty;
                 }
                 else
                 {
                     droneId = foundDrone.Id;
                     _logger.LogDebug("Found drone by index: {DroneId}", droneId);
                 }
             }

             var telemetry = JsonSerializer.Deserialize<DroneTelemetryMessage>(
                 message!,
                 new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

             if (telemetry == null)
             {
                 _logger.LogWarning("Failed to deserialize telemetry message");
                 return;
             }

             _logger.LogDebug("Telemetry parsed → lat={Lat}, lon={Lon}, battery={Battery}%, armed={Armed}, mode={Mode}",
                 telemetry.latitude, telemetry.longitude,
                 telemetry.battery_percent, telemetry.armed, telemetry.flight_mode);

             using var scope = _scopeFactory.CreateScope();
             var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

             if (droneId != Guid.Empty)
             {
                 var droneEntity = await db.Drones
                     .FirstOrDefaultAsync(d => d.Id == droneId);

                if (droneEntity != null)
                 {
                     _logger.LogDebug("Updating drone in DB: {DroneId}", droneId);

                     droneEntity.CurrentLatitude = telemetry.latitude;
                     droneEntity.CurrentLongitude = telemetry.longitude;
                     droneEntity.BatteryLevel = telemetry.battery_percent;
                    droneEntity.LastSeenAt = DateTime.UtcNow;

                    var activeDispatch = await db.Dispatches
                        .FirstOrDefaultAsync(d => d.DroneId == droneId &&
                            d.Status == DispatchStatus.InFlight);
                    if (activeDispatch != null)
                    {
                        var now = DateTime.UtcNow;
                        var lastPointAt = await db.DroneRoutePoints
                            .Where(p => p.DispatchId == activeDispatch.Id)
                            .OrderByDescending(p => p.RecordedAt)
                            .Select(p => (DateTime?)p.RecordedAt)
                            .FirstOrDefaultAsync();
                        if (!lastPointAt.HasValue || now - lastPointAt.Value >= TimeSpan.FromSeconds(5))
                        {
                            db.DroneRoutePoints.Add(new DroneRoutePoint
                            {
                                DispatchId = activeDispatch.Id,
                                DroneId = droneId,
                                Latitude = telemetry.latitude,
                                Longitude = telemetry.longitude,
                                BatteryLevel = telemetry.battery_percent,
                                RecordedAt = now
                            });
                        }
                    }

                     await db.SaveChangesAsync();
                     _logger.LogDebug("DB updated successfully for drone: {DroneId}", droneId);
                 }
                 else
                 {
                     _logger.LogWarning("Drone {DroneId} not found in DB — skipping DB update", droneId);
                 }
             }
             else
             {
                 _logger.LogWarning("Skipping DB update — droneId is empty");
             }

             _logger.LogDebug("Pushing to SignalR group: drone:{DroneIdStr}", droneIdStr);

             await _hubContext.Clients
                 .Group($"drone:{droneIdStr}")
                 .SendAsync("DroneLocationUpdated", new
                 {
                     droneId = droneIdStr,
                     latitude = telemetry.latitude,
                     longitude = telemetry.longitude,
                     batteryLevel = telemetry.battery_percent,
                     roll = telemetry.roll,
                     pitch = telemetry.pitch,
                     yaw = telemetry.yaw,
                     armed = telemetry.armed,
                     flightMode = telemetry.flight_mode,
                     satellites = telemetry.satellites,
                     errors = telemetry.errors,
                     timestamp = DateTime.UtcNow
                 });

             _logger.LogInformation(
                 "Drone {DroneId} → lat={Lat}, lon={Lon}, battery={Battery}%, armed={Armed}",
                 droneIdStr, telemetry.latitude, telemetry.longitude,
                 telemetry.battery_percent, telemetry.armed);
         }
         catch (Exception ex)
         {
             _logger.LogError(ex, "Error processing drone telemetry from channel {Channel}", channel);
         }
     });

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
    }


    public class DroneTelemetryMessage
    {
        public double battery_percent { get; set; }
        public double roll { get; set; }
        public double pitch { get; set; }
        public double yaw { get; set; }
        public int motor_1 { get; set; }
        public int motor_2 { get; set; }
        public int motor_3 { get; set; }
        public int motor_4 { get; set; }
        public bool armed { get; set; }
        public string flight_mode { get; set; } = string.Empty;
        public List<string> errors { get; set; } = [];
        public double latitude { get; set; }
        public double longitude { get; set; }
        public int satellites { get; set; }
    }
}
