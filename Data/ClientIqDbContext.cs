using Microsoft.EntityFrameworkCore;
using clientIq_api.Models;

namespace clientIq_api.Data
{
    public class ClientIqDbContext : DbContext
    {
        public ClientIqDbContext(DbContextOptions<ClientIqDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User entity
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Email)
                    .IsUnique();

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UpdatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            // Configure Client entity
            modelBuilder.Entity<Client>(entity =>
            {
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UpdatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            // Configure Product entity
            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UpdatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            // Configure Order entity
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasIndex(e => e.OrderNumber)
                    .IsUnique();

                entity.Property(e => e.OrderDate)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UpdatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                // Configure relationships
                entity.HasOne(e => e.Client)
                    .WithMany(c => c.Orders)
                    .HasForeignKey(e => e.ClientId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.CreatedByUser)
                    .WithMany(u => u.Orders)
                    .HasForeignKey(e => e.CreatedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(e => e.OrderItems)
                    .WithOne(oi => oi.Order)
                    .HasForeignKey(oi => oi.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure OrderItem entity
            modelBuilder.Entity<OrderItem>(entity =>
            {
                // Configure relationships
                entity.HasOne(e => e.Product)
                    .WithMany(p => p.OrderItems)
                    .HasForeignKey(e => e.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Seed data (optional)
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            var now = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            // Seed Users (Sales Reps)
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Email = "john.smith@clientiq.com",
                    PasswordHash = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("password123salt123")),
                    FirstName = "John",
                    LastName = "Smith",
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new User
                {
                    Id = 2,
                    Email = "sarah.johnson@clientiq.com",
                    PasswordHash = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("password123salt123")),
                    FirstName = "Sarah",
                    LastName = "Johnson",
                    CreatedAt = now,
                    UpdatedAt = now
                }
            );

            // Seed Products
            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Name = "Premium Widget",
                    Description = "High-quality widget for enterprise customers",
                    Price = 299.99m,
                    IsActive = true,
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new Product
                {
                    Id = 2,
                    Name = "Standard Widget",
                    Description = "Basic widget for small businesses",
                    Price = 99.99m,
                    IsActive = true,
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new Product
                {
                    Id = 3,
                    Name = "Widget Pro",
                    Description = "Professional grade widget with advanced features",
                    Price = 499.99m,
                    IsActive = true,
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new Product
                {
                    Id = 4,
                    Name = "Widget Lite",
                    Description = "Entry-level widget for startups",
                    Price = 49.99m,
                    IsActive = true,
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new Product
                {
                    Id = 5,
                    Name = "Widget Enterprise",
                    Description = "Enterprise-grade widget with full support",
                    Price = 999.99m,
                    IsActive = true,
                    CreatedAt = now,
                    UpdatedAt = now
                }
            );

            // Seed Clients
            modelBuilder.Entity<Client>().HasData(
                new Client
                {
                    Id = 1,
                    CompanyName = "Acme Corporation",
                    ContactName = "Bob Anderson",
                    Email = "bob.anderson@acme.com",
                    Phone = "+1-555-0123",
                    Address = "123 Business Ave, Suite 100, New York, NY 10001",
                    Notes = "Large enterprise client, prefers bulk orders",
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new Client
                {
                    Id = 2,
                    CompanyName = "TechStart Inc",
                    ContactName = "Alice Chen",
                    Email = "alice@techstart.io",
                    Phone = "+1-555-0456",
                    Address = "456 Innovation Dr, San Francisco, CA 94105",
                    Notes = "Growing startup, price-sensitive",
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new Client
                {
                    Id = 3,
                    CompanyName = "Global Solutions Ltd",
                    ContactName = "Michael Davis",
                    Email = "m.davis@globalsolutions.com",
                    Phone = "+1-555-0789",
                    Address = "789 Corporate Blvd, Chicago, IL 60601",
                    Notes = "International client, requires special shipping",
                    CreatedAt = now,
                    UpdatedAt = now
                }
            );

            // Seed Orders
            modelBuilder.Entity<Order>().HasData(
                new Order
                {
                    Id = 1,
                    OrderNumber = "ORD-2024-001",
                    ClientId = 1,
                    OrderDate = now.AddDays(-30),
                    DueDate = now.AddDays(-15),
                    Status = "Completed",
                    PaymentStatus = "Paid",
                    TotalAmount = 899.97m,
                    Notes = "Bulk order with discount applied",
                    CreatedByUserId = 1,
                    CreatedAt = now.AddDays(-30),
                    UpdatedAt = now.AddDays(-15)
                },
                new Order
                {
                    Id = 2,
                    OrderNumber = "ORD-2024-002",
                    ClientId = 2,
                    OrderDate = now.AddDays(-15),
                    DueDate = now.AddDays(5),
                    Status = "InProgress",
                    PaymentStatus = "Partial",
                    TotalAmount = 149.98m,
                    Notes = "Startup discount applied",
                    CreatedByUserId = 2,
                    CreatedAt = now.AddDays(-15),
                    UpdatedAt = now.AddDays(-5)
                },
                new Order
                {
                    Id = 3,
                    OrderNumber = "ORD-2024-003",
                    ClientId = 3,
                    OrderDate = now.AddDays(-7),
                    DueDate = now.AddDays(14),
                    Status = "New",
                    PaymentStatus = "Unpaid",
                    TotalAmount = 1999.97m,
                    Notes = "International shipping required",
                    CreatedByUserId = 1,
                    CreatedAt = now.AddDays(-7),
                    UpdatedAt = now.AddDays(-7)
                }
            );

            // Seed Order Items
            modelBuilder.Entity<OrderItem>().HasData(
                // Order 1 items
                new OrderItem
                {
                    Id = 1,
                    OrderId = 1,
                    ProductId = 1, // Premium Widget
                    Quantity = 2,
                    UnitPrice = 299.99m,
                    Subtotal = 599.98m
                },
                new OrderItem
                {
                    Id = 2,
                    OrderId = 1,
                    ProductId = 2, // Standard Widget
                    Quantity = 3,
                    UnitPrice = 99.99m,
                    Subtotal = 299.97m
                },

                // Order 2 items
                new OrderItem
                {
                    Id = 3,
                    OrderId = 2,
                    ProductId = 4, // Widget Lite
                    Quantity = 1,
                    UnitPrice = 49.99m,
                    Subtotal = 49.99m
                },
                new OrderItem
                {
                    Id = 4,
                    OrderId = 2,
                    ProductId = 2, // Standard Widget
                    Quantity = 1,
                    UnitPrice = 99.99m,
                    Subtotal = 99.99m
                },

                // Order 3 items
                new OrderItem
                {
                    Id = 5,
                    OrderId = 3,
                    ProductId = 5, // Widget Enterprise
                    Quantity = 2,
                    UnitPrice = 999.99m,
                    Subtotal = 1999.98m
                }
            );
        }
    }
}