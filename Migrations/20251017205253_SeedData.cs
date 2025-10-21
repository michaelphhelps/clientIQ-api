using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace clientIq_api.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Clients",
                columns: new[] { "Id", "Address", "CompanyName", "ContactName", "CreatedAt", "Email", "Notes", "Phone", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, "123 Business Ave, Suite 100, New York, NY 10001", "Acme Corporation", "Bob Anderson", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "bob.anderson@acme.com", "Large enterprise client, prefers bulk orders", "+1-555-0123", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 2, "456 Innovation Dr, San Francisco, CA 94105", "TechStart Inc", "Alice Chen", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "alice@techstart.io", "Growing startup, price-sensitive", "+1-555-0456", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3, "789 Corporate Blvd, Chicago, IL 60601", "Global Solutions Ltd", "Michael Davis", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "m.davis@globalsolutions.com", "International client, requires special shipping", "+1-555-0789", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CreatedAt", "Description", "IsActive", "Name", "Price", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "High-quality widget for enterprise customers", true, "Premium Widget", 299.99m, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Basic widget for small businesses", true, "Standard Widget", 99.99m, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Professional grade widget with advanced features", true, "Widget Pro", 499.99m, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 4, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Entry-level widget for startups", true, "Widget Lite", 49.99m, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 5, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Enterprise-grade widget with full support", true, "Widget Enterprise", 999.99m, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "FirstName", "LastName", "PasswordHash", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "john.smith@clientiq.com", "John", "Smith", "cGFzc3dvcmQxMjNzYWx0MTIz", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "sarah.johnson@clientiq.com", "Sarah", "Johnson", "cGFzc3dvcmQxMjNzYWx0MTIz", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "Orders",
                columns: new[] { "Id", "ClientId", "CreatedAt", "CreatedByUserId", "DueDate", "Notes", "OrderDate", "OrderNumber", "PaymentStatus", "Status", "TotalAmount", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2023, 12, 2, 0, 0, 0, 0, DateTimeKind.Utc), 1, new DateTime(2023, 12, 17, 0, 0, 0, 0, DateTimeKind.Utc), "Bulk order with discount applied", new DateTime(2023, 12, 2, 0, 0, 0, 0, DateTimeKind.Utc), "ORD-2024-001", "Paid", "Completed", 899.97m, new DateTime(2023, 12, 17, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 2, 2, new DateTime(2023, 12, 17, 0, 0, 0, 0, DateTimeKind.Utc), 2, new DateTime(2024, 1, 6, 0, 0, 0, 0, DateTimeKind.Utc), "Startup discount applied", new DateTime(2023, 12, 17, 0, 0, 0, 0, DateTimeKind.Utc), "ORD-2024-002", "Partial", "InProgress", 149.98m, new DateTime(2023, 12, 27, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3, 3, new DateTime(2023, 12, 25, 0, 0, 0, 0, DateTimeKind.Utc), 1, new DateTime(2024, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), "International shipping required", new DateTime(2023, 12, 25, 0, 0, 0, 0, DateTimeKind.Utc), "ORD-2024-003", "Unpaid", "New", 1999.97m, new DateTime(2023, 12, 25, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "OrderItems",
                columns: new[] { "Id", "OrderId", "ProductId", "Quantity", "Subtotal", "UnitPrice" },
                values: new object[,]
                {
                    { 1, 1, 1, 2, 599.98m, 299.99m },
                    { 2, 1, 2, 3, 299.97m, 99.99m },
                    { 3, 2, 4, 1, 49.99m, 49.99m },
                    { 4, 2, 2, 1, 99.99m, 99.99m },
                    { 5, 3, 5, 2, 1999.98m, 999.99m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "OrderItems",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "OrderItems",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "OrderItems",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "OrderItems",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "OrderItems",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
