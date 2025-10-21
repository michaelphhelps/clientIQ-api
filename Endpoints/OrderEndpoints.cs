using Microsoft.EntityFrameworkCore;
using clientIq_api.Data;
using clientIq_api.Models;
using clientIq_api.DTO;

namespace clientIq_api.Endpoints
{
    public static class OrderEndpoints
    {
        public static void MapOrderEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/api/orders")
                .WithTags("Orders");

            // Helper method to map Order entity to OrderResponseDto
            static OrderResponseDto MapToOrderResponseDto(Order order)
            {
                return new OrderResponseDto
                {
                    Id = order.Id,
                    OrderNumber = order.OrderNumber,
                    ClientId = order.ClientId,
                    ClientCompanyName = order.Client?.CompanyName ?? "Unknown Client",
                    OrderDate = order.OrderDate,
                    DueDate = order.DueDate,
                    Status = order.Status,
                    PaymentStatus = order.PaymentStatus,
                    TotalAmount = order.TotalAmount,
                    Notes = order.Notes,
                    CreatedByUserId = order.CreatedByUserId,
                    CreatedByUserName = order.CreatedByUser != null
                        ? $"{order.CreatedByUser.FirstName} {order.CreatedByUser.LastName}"
                        : "Unknown User",
                    CreatedAt = order.CreatedAt,
                    UpdatedAt = order.UpdatedAt,
                    OrderItems = order.OrderItems?.Select(oi => new OrderItemResponseDto
                    {
                        Id = oi.Id,
                        OrderId = oi.OrderId,
                        ProductId = oi.ProductId,
                        ProductName = oi.Product?.Name ?? "Unknown Product",
                        Quantity = oi.Quantity,
                        UnitPrice = oi.UnitPrice,
                        Subtotal = oi.Subtotal
                    }).ToList() ?? new List<OrderItemResponseDto>()
                };
            }

            // GET /api/orders - Get all orders
            group.MapGet("/", async (ClientIqDbContext context) =>
            {
                var orders = await context.Orders
                    .Include(o => o.Client)
                    .Include(o => o.CreatedByUser)
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                    .ToListAsync();

                return Results.Ok(orders.Select(MapToOrderResponseDto));
            })
            .WithName("GetOrders")
            .WithSummary("Get all orders");

            // GET /api/orders/{id} - Get order by ID
            group.MapGet("/{id:int}", async (int id, ClientIqDbContext context) =>
            {
                var order = await context.Orders
                    .Include(o => o.Client)
                    .Include(o => o.CreatedByUser)
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                    .FirstOrDefaultAsync(o => o.Id == id);

                if (order == null)
                {
                    return Results.NotFound();
                }

                return Results.Ok(MapToOrderResponseDto(order));
            })
            .WithName("GetOrder")
            .WithSummary("Get order by ID");

            // POST /api/orders - Create new order
            group.MapPost("/", async (OrderCreateDto orderCreateDto, ClientIqDbContext context) =>
            {
                // Validate client exists
                var client = await context.Clients.FindAsync(orderCreateDto.ClientId);
                if (client == null)
                {
                    return Results.BadRequest("Client not found.");
                }

                // Check if order number already exists
                var existingOrder = await context.Orders
                    .FirstOrDefaultAsync(o => o.OrderNumber == orderCreateDto.OrderNumber);

                if (existingOrder != null)
                {
                    return Results.BadRequest("An order with this order number already exists.");
                }

                // Create the order
                var order = new Order
                {
                    OrderNumber = orderCreateDto.OrderNumber,
                    ClientId = orderCreateDto.ClientId,
                    OrderDate = DateTime.UtcNow,
                    DueDate = orderCreateDto.DueDate,
                    Status = orderCreateDto.Status ?? "New",
                    PaymentStatus = orderCreateDto.PaymentStatus ?? "Unpaid",
                    TotalAmount = 0, // Will be calculated from order items
                    Notes = orderCreateDto.Notes,
                    CreatedByUserId = 1, // TODO: Get from authentication context
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                context.Orders.Add(order);
                await context.SaveChangesAsync();

                // Add order items and calculate total
                decimal totalAmount = 0;
                if (orderCreateDto.OrderItems != null)
                {
                    foreach (var itemDto in orderCreateDto.OrderItems)
                    {
                        var product = await context.Products.FindAsync(itemDto.ProductId);
                        if (product == null)
                        {
                            return Results.BadRequest($"Product with ID {itemDto.ProductId} not found.");
                        }

                        var subtotal = itemDto.Quantity * itemDto.UnitPrice;
                        totalAmount += subtotal;

                        var orderItem = new OrderItem
                        {
                            OrderId = order.Id,
                            ProductId = itemDto.ProductId,
                            Quantity = itemDto.Quantity,
                            UnitPrice = itemDto.UnitPrice,
                            Subtotal = subtotal
                        };

                        context.OrderItems.Add(orderItem);
                    }
                }

                // Update order total
                order.TotalAmount = totalAmount;
                await context.SaveChangesAsync();

                // Return the created order with all details
                var createdOrder = await context.Orders
                    .Include(o => o.Client)
                    .Include(o => o.CreatedByUser)
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                    .FirstOrDefaultAsync(o => o.Id == order.Id);

                return Results.Created($"/api/orders/{order.Id}", MapToOrderResponseDto(createdOrder!));
            })
            .WithName("CreateOrder")
            .WithSummary("Create a new order with items");

            // PUT /api/orders/{id} - Update order
            group.MapPut("/{id:int}", async (int id, OrderUpdateDto orderUpdateDto, ClientIqDbContext context) =>
            {
                var order = await context.Orders
                    .Include(o => o.OrderItems)
                    .FirstOrDefaultAsync(o => o.Id == id);

                if (order == null)
                {
                    return Results.NotFound();
                }

                if (orderUpdateDto.ClientId.HasValue)
                {
                    var client = await context.Clients.FindAsync(orderUpdateDto.ClientId.Value);
                    if (client == null)
                    {
                        return Results.BadRequest("Client not found.");
                    }
                    order.ClientId = orderUpdateDto.ClientId.Value;
                }

                if (orderUpdateDto.DueDate.HasValue)
                    order.DueDate = orderUpdateDto.DueDate.Value;

                if (!string.IsNullOrEmpty(orderUpdateDto.Status))
                    order.Status = orderUpdateDto.Status;

                if (!string.IsNullOrEmpty(orderUpdateDto.PaymentStatus))
                    order.PaymentStatus = orderUpdateDto.PaymentStatus;

                if (orderUpdateDto.Notes != null)
                    order.Notes = orderUpdateDto.Notes;

                // Handle OrderItems updates
                if (orderUpdateDto.OrderItems != null)
                {
                    // Remove existing order items
                    context.OrderItems.RemoveRange(order.OrderItems);

                    // Add new order items and calculate total
                    decimal totalAmount = 0;
                    foreach (var itemDto in orderUpdateDto.OrderItems)
                    {
                        var product = await context.Products.FindAsync(itemDto.ProductId);
                        if (product == null)
                        {
                            return Results.BadRequest($"Product with ID {itemDto.ProductId} not found.");
                        }

                        var subtotal = itemDto.Quantity * itemDto.UnitPrice;
                        totalAmount += subtotal;

                        var orderItem = new OrderItem
                        {
                            OrderId = order.Id,
                            ProductId = itemDto.ProductId,
                            Quantity = itemDto.Quantity,
                            UnitPrice = itemDto.UnitPrice,
                            Subtotal = subtotal
                        };

                        context.OrderItems.Add(orderItem);
                    }

                    // Update order total
                    order.TotalAmount = totalAmount;
                }

                order.UpdatedAt = DateTime.UtcNow;

                try
                {
                    await context.SaveChangesAsync();
                    return Results.NoContent();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await context.Orders.AnyAsync(o => o.Id == id))
                    {
                        return Results.NotFound();
                    }
                    throw;
                }
            })
            .WithName("UpdateOrder")
            .WithSummary("Update an existing order");

            // DELETE /api/orders/{id} - Delete order
            group.MapDelete("/{id:int}", async (int id, ClientIqDbContext context) =>
            {
                var order = await context.Orders
                    .Include(o => o.OrderItems)
                    .FirstOrDefaultAsync(o => o.Id == id);

                if (order == null)
                {
                    return Results.NotFound();
                }

                // Remove all order items first (cascade delete should handle this, but being explicit)
                context.OrderItems.RemoveRange(order.OrderItems);
                context.Orders.Remove(order);
                await context.SaveChangesAsync();

                return Results.NoContent();
            })
            .WithName("DeleteOrder")
            .WithSummary("Delete an order and all its items");
        }
    }
}