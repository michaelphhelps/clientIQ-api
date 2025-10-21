using Microsoft.EntityFrameworkCore;
using clientIq_api.Data;
using clientIq_api.Models;
using clientIq_api.DTO;

namespace clientIq_api.Endpoints
{
    public static class ProductEndpoints
    {
        public static void MapProductEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/api/products")
                .WithTags("Products");

            // GET /api/products - Get all products
            group.MapGet("/", async (ClientIqDbContext context) =>
            {
                var products = await context.Products
                    .Select(p => new ProductResponseDto
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description,
                        Price = p.Price,
                        IsActive = p.IsActive,
                        CreatedAt = p.CreatedAt,
                        UpdatedAt = p.UpdatedAt
                    })
                    .ToListAsync();

                return Results.Ok(products);
            })
            .WithName("GetProducts")
            .WithSummary("Get all products");

            // GET /api/products/{id} - Get product by ID
            group.MapGet("/{id:int}", async (int id, ClientIqDbContext context) =>
            {
                var product = await context.Products.FindAsync(id);

                if (product == null)
                {
                    return Results.NotFound();
                }

                var productDto = new ProductResponseDto
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    IsActive = product.IsActive,
                    CreatedAt = product.CreatedAt,
                    UpdatedAt = product.UpdatedAt
                };

                return Results.Ok(productDto);
            })
            .WithName("GetProduct")
            .WithSummary("Get product by ID");

            // POST /api/products - Create new product
            group.MapPost("/", async (ProductCreateDto productCreateDto, ClientIqDbContext context) =>
            {
                var product = new Product
                {
                    Name = productCreateDto.Name,
                    Description = productCreateDto.Description,
                    Price = productCreateDto.Price,
                    IsActive = productCreateDto.IsActive,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                context.Products.Add(product);
                await context.SaveChangesAsync();

                var productDto = new ProductResponseDto
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    IsActive = product.IsActive,
                    CreatedAt = product.CreatedAt,
                    UpdatedAt = product.UpdatedAt
                };

                return Results.Created($"/api/products/{product.Id}", productDto);
            })
            .WithName("CreateProduct")
            .WithSummary("Create a new product");

            // PUT /api/products/{id} - Update product
            group.MapPut("/{id:int}", async (int id, ProductUpdateDto productUpdateDto, ClientIqDbContext context) =>
            {
                var product = await context.Products.FindAsync(id);

                if (product == null)
                {
                    return Results.NotFound();
                }

                if (!string.IsNullOrEmpty(productUpdateDto.Name))
                    product.Name = productUpdateDto.Name;

                if (productUpdateDto.Description != null)
                    product.Description = productUpdateDto.Description;

                if (productUpdateDto.Price.HasValue)
                    product.Price = productUpdateDto.Price.Value;

                if (productUpdateDto.IsActive.HasValue)
                    product.IsActive = productUpdateDto.IsActive.Value;

                product.UpdatedAt = DateTime.UtcNow;

                try
                {
                    await context.SaveChangesAsync();
                    return Results.NoContent();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await context.Products.AnyAsync(p => p.Id == id))
                    {
                        return Results.NotFound();
                    }
                    throw;
                }
            })
            .WithName("UpdateProduct")
            .WithSummary("Update an existing product");

            // DELETE /api/products/{id} - Delete product
            group.MapDelete("/{id:int}", async (int id, ClientIqDbContext context) =>
            {
                var product = await context.Products.FindAsync(id);
                if (product == null)
                {
                    return Results.NotFound();
                }

                context.Products.Remove(product);
                await context.SaveChangesAsync();

                return Results.NoContent();
            })
            .WithName("DeleteProduct")
            .WithSummary("Delete a product");
        }
    }
}