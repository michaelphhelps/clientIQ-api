using System.ComponentModel.DataAnnotations;

namespace clientIq_api.DTO
{
    public class OrderCreateDto
    {
        [Required]
        [StringLength(50)]
        public string OrderNumber { get; set; } = string.Empty;

        [Required]
        public int ClientId { get; set; }

        public DateTime? DueDate { get; set; }

        [StringLength(50)]
        public string? Status { get; set; } = "New";

        [StringLength(50)]
        public string? PaymentStatus { get; set; } = "Unpaid";

        public string? Notes { get; set; }

        public List<OrderItemCreateDto> OrderItems { get; set; } = new List<OrderItemCreateDto>();
    }

    public class OrderResponseDto
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public int ClientId { get; set; }
        public string ClientCompanyName { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public DateTime? DueDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string? Notes { get; set; }
        public int CreatedByUserId { get; set; }
        public string CreatedByUserName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<OrderItemResponseDto> OrderItems { get; set; } = new List<OrderItemResponseDto>();
    }

    public class OrderUpdateDto
    {
        public int? ClientId { get; set; }
        public DateTime? DueDate { get; set; }

        [StringLength(50)]
        public string? Status { get; set; }

        [StringLength(50)]
        public string? PaymentStatus { get; set; }

        public string? Notes { get; set; }

        // Add this to allow updating order items
        public List<OrderItemCreateDto>? OrderItems { get; set; }
    }

    public class OrderItemCreateDto
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; } = 1;

        [Required]
        [Range(0.01, 999999.99)]
        public decimal UnitPrice { get; set; }
    }

    public class OrderItemResponseDto
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Subtotal { get; set; }
    }
}