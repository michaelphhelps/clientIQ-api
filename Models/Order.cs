using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace clientIq_api.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string OrderNumber { get; set; } = string.Empty;

        [Required]
        [ForeignKey("Client")]
        public int ClientId { get; set; }

        [Required]
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        public DateTime? DueDate { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "New"; // New, InProgress, ReadyForDelivery, Completed, Cancelled

        [Required]
        [StringLength(50)]
        public string PaymentStatus { get; set; } = "Unpaid"; // Unpaid, Partial, Paid

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalAmount { get; set; } = 0.00m;

        public string? Notes { get; set; }

        [Required]
        [ForeignKey("CreatedByUser")]
        public int CreatedByUserId { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual Client Client { get; set; } = null!;
        public virtual User CreatedByUser { get; set; } = null!;
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}