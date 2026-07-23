using Drones.src.Api.Auth.Entities;
using Drones.src.Api.DeliveryPoints.Entities;
using Drones.src.Api.Dispatches.Entities;
using Drones.src.Api.Drones.Entities;
using Drones.src.Api.Orders.Entities;
using Drones.src.Api.Payments.Entities;
using Drones.src.Api.Products.Entities;
using Drones.src.Api.Restaurants.Entities;
using Microsoft.EntityFrameworkCore;

namespace Drones.src.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        //Auth
        public DbSet<User> Users => Set<User>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<UserRole> UserRoles => Set<UserRole>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        public DbSet<UserVerficationToken> UserVerficationTokens => Set<UserVerficationToken>();
        public DbSet<PasswordResetToken> PasswordResetTokens => Set<PasswordResetToken>();

        //Orders
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();

        //Payments
        public DbSet<Payment> Payments => Set<Payment>();

        //Restaurants
        public DbSet<Review> Reviews => Set<Review>();
        public DbSet<Restaurant> Restaurants => Set<Restaurant>();
        public DbSet<RestaurantApplication> RestaurantApplications => Set<RestaurantApplication>();

        //Products
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Category> Categories => Set<Category>();   

        //Drones
        public DbSet<Drone> Drones => Set<Drone>();
        public DbSet<DroneMaintenanceLog> DroneMaintenanceLogs => Set<DroneMaintenanceLog>();
        public DbSet<DroneRoutePoint> DroneRoutePoints => Set<DroneRoutePoint>();

        //Dispatch
        public DbSet<Dispatch> Dispatches => Set<Dispatch>();

        //DeliveryPoints
        public DbSet<DeliveryPoint> DeliveryPoints => Set<DeliveryPoint>();


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
            SeedData(modelBuilder);
        }

        private static void SeedData(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "Admin", Description = "Full access to all resources." },
                new Role { Id = 2, Name = "User", Description = "Limited access to own resources." },
                new Role { Id = 3, Name = "RestaurantOwner", Description = "Manages an approved restaurant." }
            );

            var adminId = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890");
            var customerId = Guid.Parse("b2c3d4e5-f6a7-8901-bcde-f12345678901");
            var seededAt = new DateTime(2026, 6, 5, 0, 0, 0, DateTimeKind.Utc);

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = adminId,
                    Email = "admin@drones.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                    IsEmailVerified = true,
                    IsActive = true,
                    CreatedAt = seededAt,
                },
                new User
                {
                    Id = customerId,
                    Email = "customer@drones.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Customer123!"),
                    IsEmailVerified = true,
                    IsActive = true,
                    CreatedAt = seededAt,
                });

            modelBuilder.Entity<UserRole>().HasData(
                new UserRole
                {
                    UserId = adminId,
                    RoleId = 1,
                    AssignedAt = seededAt,
                },
                new UserRole
                {
                    UserId = customerId,
                    RoleId = 2,
                    AssignedAt = seededAt,
                });

            var point1Id = Guid.Parse("c1d2e3f4-a5b6-7890-cdef-123456789abc");
            var point2Id = Guid.Parse("d2e3f4a5-b6c7-8901-defa-234567890bcd");
            var point3Id = Guid.Parse("e3f4a5b6-c7d8-9012-efab-345678901cde");

            modelBuilder.Entity<DeliveryPoint>().HasData(
                new DeliveryPoint
                {
                    Id = point1Id,
                    Name = "Warsaw Central",
                    Address = "Al. Jerozolimskie 54, Warsaw",
                    Latitude = 52.2297,
                    Longitude = 21.0122,
                    IsActive = true,
                    CreatedAt = seededAt,
                },
                new DeliveryPoint
                {
                    Id = point2Id,
                    Name = "University Campus",
                    Address = "Krakowskie Przedmieście 26/28, Warsaw",
                    Latitude = 52.2411,
                    Longitude = 21.0185,
                    IsActive = true,
                    CreatedAt = seededAt,
                },
                new DeliveryPoint
                {
                    Id = point3Id,
                    Name = "Mokotów Park",
                    Address = "Wołoska 12, Warsaw",
                    Latitude = 52.1934,
                    Longitude = 21.0345,
                    IsActive = true,
                    CreatedAt = seededAt,
                });
        }
    }
}
