using Microsoft.EntityFrameworkCore;
using clientIq_api.Data;
using clientIq_api.Models;
using clientIq_api.DTO;

namespace clientIq_api.Endpoints
{
    public static class UserEndpoints
    {
        public static void MapUserEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/api/users")
                .WithTags("Users");

            // GET /api/users - Get all users
            group.MapGet("/", async (ClientIqDbContext context) =>
            {
                var users = await context.Users
                    .Select(u => new UserResponseDto
                    {
                        Id = u.Id,
                        Email = u.Email,
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        CreatedAt = u.CreatedAt,
                        UpdatedAt = u.UpdatedAt
                    })
                    .ToListAsync();

                return Results.Ok(users);
            })
            .WithName("GetUsers")
            .WithSummary("Get all users");

            // GET /api/users/{id} - Get user by ID
            group.MapGet("/{id:int}", async (int id, ClientIqDbContext context) =>
            {
                var user = await context.Users.FindAsync(id);

                if (user == null)
                {
                    return Results.NotFound();
                }

                var userDto = new UserResponseDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = user.UpdatedAt
                };

                return Results.Ok(userDto);
            })
            .WithName("GetUser")
            .WithSummary("Get user by ID");

            // GET /api/users/{id}/orders - Get all orders created by a user
            group.MapGet("/{id:int}/orders", async (int id, ClientIqDbContext context) =>
            {
                var user = await context.Users.FindAsync(id);
                if (user == null)
                {
                    return Results.NotFound();
                }

                var orders = await context.Orders
                    .Where(o => o.CreatedByUserId == id)
                    .Include(o => o.Client)
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                    .Select(o => new OrderResponseDto
                    {
                        Id = o.Id,
                        OrderNumber = o.OrderNumber,
                        ClientId = o.ClientId,
                        ClientCompanyName = o.Client.CompanyName,
                        OrderDate = o.OrderDate,
                        DueDate = o.DueDate,
                        Status = o.Status,
                        PaymentStatus = o.PaymentStatus,
                        TotalAmount = o.TotalAmount,
                        Notes = o.Notes,
                        CreatedByUserId = o.CreatedByUserId,
                        CreatedByUserName = $"{o.CreatedByUser.FirstName} {o.CreatedByUser.LastName}",
                        CreatedAt = o.CreatedAt,
                        UpdatedAt = o.UpdatedAt,
                        OrderItems = o.OrderItems.Select(oi => new OrderItemResponseDto
                        {
                            Id = oi.Id,
                            OrderId = oi.OrderId,
                            ProductId = oi.ProductId,
                            ProductName = oi.Product.Name,
                            Quantity = oi.Quantity,
                            UnitPrice = oi.UnitPrice,
                            Subtotal = oi.Subtotal
                        }).ToList()
                    })
                    .ToListAsync();

                return Results.Ok(orders);
            })
            .WithName("GetUserOrders")
            .WithSummary("Get all orders created by a specific user");

            // POST /api/users/login - User login
            group.MapPost("/login", async (UserLoginDto loginDto, ClientIqDbContext context) =>
            {
                // Find user by email
                var user = await context.Users
                    .FirstOrDefaultAsync(u => u.Email == loginDto.Email);

                if (user == null)
                {
                    return Results.BadRequest("Invalid email or password.");
                }

                // Verify password (using same simple hashing as registration)
                var expectedPasswordHash = Convert.ToBase64String(
                    System.Text.Encoding.UTF8.GetBytes(loginDto.Password + "salt123"));

                if (user.PasswordHash != expectedPasswordHash)
                {
                    return Results.BadRequest("Invalid email or password.");
                }

                // Return user data (without password)
                var userResponse = new UserResponseDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = user.UpdatedAt
                };

                return Results.Ok(userResponse);
            })
            .WithName("LoginUser")
            .WithSummary("User login");

            // POST /api/users - Create new user
            group.MapPost("/", async (UserCreateDto userCreateDto, ClientIqDbContext context) =>
            {
                // Check if email already exists
                var existingUser = await context.Users
                    .FirstOrDefaultAsync(u => u.Email == userCreateDto.Email);

                if (existingUser != null)
                {
                    return Results.BadRequest("A user with this email already exists.");
                }

                // Simple password hashing for demo purposes
                // In production, use BCrypt or ASP.NET Identity
                var passwordHash = Convert.ToBase64String(
                    System.Text.Encoding.UTF8.GetBytes(userCreateDto.Password + "salt123"));

                var user = new User
                {
                    Email = userCreateDto.Email,
                    PasswordHash = passwordHash,
                    FirstName = userCreateDto.FirstName,
                    LastName = userCreateDto.LastName,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                context.Users.Add(user);
                await context.SaveChangesAsync();

                var userDto = new UserResponseDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = user.UpdatedAt
                };

                return Results.Created($"/api/users/{user.Id}", userDto);
            })
            .WithName("CreateUser")
            .WithSummary("Create a new user");

            // PUT /api/users/{id} - Update user
            group.MapPut("/{id:int}", async (int id, UserUpdateDto userUpdateDto, ClientIqDbContext context) =>
            {
                var user = await context.Users.FindAsync(id);

                if (user == null)
                {
                    return Results.NotFound();
                }

                if (!string.IsNullOrEmpty(userUpdateDto.Email))
                {
                    // Check if new email already exists
                    var existingUser = await context.Users
                        .FirstOrDefaultAsync(u => u.Email == userUpdateDto.Email && u.Id != id);

                    if (existingUser != null)
                    {
                        return Results.BadRequest("A user with this email already exists.");
                    }

                    user.Email = userUpdateDto.Email;
                }

                if (!string.IsNullOrEmpty(userUpdateDto.FirstName))
                    user.FirstName = userUpdateDto.FirstName;

                if (!string.IsNullOrEmpty(userUpdateDto.LastName))
                    user.LastName = userUpdateDto.LastName;

                user.UpdatedAt = DateTime.UtcNow;

                try
                {
                    await context.SaveChangesAsync();
                    return Results.NoContent();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await context.Users.AnyAsync(u => u.Id == id))
                    {
                        return Results.NotFound();
                    }
                    throw;
                }
            })
            .WithName("UpdateUser")
            .WithSummary("Update an existing user");

            // DELETE /api/users/{id} - Delete user
            group.MapDelete("/{id:int}", async (int id, ClientIqDbContext context) =>
            {
                var user = await context.Users.FindAsync(id);
                if (user == null)
                {
                    return Results.NotFound();
                }

                // Check if user has created orders
                var hasOrders = await context.Orders.AnyAsync(o => o.CreatedByUserId == id);
                if (hasOrders)
                {
                    return Results.BadRequest("Cannot delete user who has created orders.");
                }

                context.Users.Remove(user);
                await context.SaveChangesAsync();

                return Results.NoContent();
            })
            .WithName("DeleteUser")
            .WithSummary("Delete a user");
        }
    }
}