using Microsoft.EntityFrameworkCore;
using clientIq_api.Data;
using clientIq_api.Models;
using clientIq_api.DTO;

namespace clientIq_api.Endpoints
{
    public static class ClientEndpoints
    {
        public static void MapClientEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/api/clients")
                .WithTags("Clients");

            // GET /api/clients - Get all clients
            group.MapGet("/", async (ClientIqDbContext context) =>
            {
                var clients = await context.Clients
                    .Select(c => new ClientResponseDto
                    {
                        Id = c.Id,
                        CompanyName = c.CompanyName,
                        ContactName = c.ContactName,
                        Email = c.Email,
                        Phone = c.Phone,
                        Address = c.Address,
                        Notes = c.Notes,
                        CreatedAt = c.CreatedAt,
                        UpdatedAt = c.UpdatedAt
                    })
                    .ToListAsync();

                return Results.Ok(clients);
            })
            .WithName("GetClients")
            .WithSummary("Get all clients");

            // GET /api/clients/{id} - Get client by ID
            group.MapGet("/{id:int}", async (int id, ClientIqDbContext context) =>
            {
                var client = await context.Clients.FindAsync(id);

                if (client == null)
                {
                    return Results.NotFound();
                }

                var clientDto = new ClientResponseDto
                {
                    Id = client.Id,
                    CompanyName = client.CompanyName,
                    ContactName = client.ContactName,
                    Email = client.Email,
                    Phone = client.Phone,
                    Address = client.Address,
                    Notes = client.Notes,
                    CreatedAt = client.CreatedAt,
                    UpdatedAt = client.UpdatedAt
                };

                return Results.Ok(clientDto);
            })
            .WithName("GetClient")
            .WithSummary("Get client by ID");

            // GET /api/clients/{id}/orders - Get all orders for a client
            group.MapGet("/{id:int}/orders", async (int id, ClientIqDbContext context) =>
            {
                var client = await context.Clients.FindAsync(id);
                if (client == null)
                {
                    return Results.NotFound();
                }

                var orders = await context.Orders
                    .Where(o => o.ClientId == id)
                    .Include(o => o.CreatedByUser)
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
            .WithName("GetClientOrders")
            .WithSummary("Get all orders for a specific client");

            // POST /api/clients - Create new client
            group.MapPost("/", async (ClientCreateDto clientCreateDto, ClientIqDbContext context) =>
            {
                var client = new Client
                {
                    CompanyName = clientCreateDto.CompanyName,
                    ContactName = clientCreateDto.ContactName,
                    Email = clientCreateDto.Email,
                    Phone = clientCreateDto.Phone,
                    Address = clientCreateDto.Address,
                    Notes = clientCreateDto.Notes,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                context.Clients.Add(client);
                await context.SaveChangesAsync();

                var clientDto = new ClientResponseDto
                {
                    Id = client.Id,
                    CompanyName = client.CompanyName,
                    ContactName = client.ContactName,
                    Email = client.Email,
                    Phone = client.Phone,
                    Address = client.Address,
                    Notes = client.Notes,
                    CreatedAt = client.CreatedAt,
                    UpdatedAt = client.UpdatedAt
                };

                return Results.Created($"/api/clients/{client.Id}", clientDto);
            })
            .WithName("CreateClient")
            .WithSummary("Create a new client");

            // PUT /api/clients/{id} - Update client
            group.MapPut("/{id:int}", async (int id, ClientUpdateDto clientUpdateDto, ClientIqDbContext context) =>
            {
                var client = await context.Clients.FindAsync(id);

                if (client == null)
                {
                    return Results.NotFound();
                }

                if (!string.IsNullOrEmpty(clientUpdateDto.CompanyName))
                    client.CompanyName = clientUpdateDto.CompanyName;

                if (!string.IsNullOrEmpty(clientUpdateDto.ContactName))
                    client.ContactName = clientUpdateDto.ContactName;

                if (clientUpdateDto.Email != null)
                    client.Email = clientUpdateDto.Email;

                if (clientUpdateDto.Phone != null)
                    client.Phone = clientUpdateDto.Phone;

                if (clientUpdateDto.Address != null)
                    client.Address = clientUpdateDto.Address;

                if (clientUpdateDto.Notes != null)
                    client.Notes = clientUpdateDto.Notes;

                client.UpdatedAt = DateTime.UtcNow;

                try
                {
                    await context.SaveChangesAsync();
                    return Results.NoContent();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await context.Clients.AnyAsync(c => c.Id == id))
                    {
                        return Results.NotFound();
                    }
                    throw;
                }
            })
            .WithName("UpdateClient")
            .WithSummary("Update an existing client");

            // DELETE /api/clients/{id} - Delete client
            group.MapDelete("/{id:int}", async (int id, ClientIqDbContext context) =>
            {
                var client = await context.Clients.FindAsync(id);
                if (client == null)
                {
                    return Results.NotFound();
                }

                // Check if client has orders
                var hasOrders = await context.Orders.AnyAsync(o => o.ClientId == id);
                if (hasOrders)
                {
                    return Results.BadRequest("Cannot delete client with existing orders.");
                }

                context.Clients.Remove(client);
                await context.SaveChangesAsync();

                return Results.NoContent();
            })
            .WithName("DeleteClient")
            .WithSummary("Delete a client");
        }
    }
}