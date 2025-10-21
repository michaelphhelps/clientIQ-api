using System.ComponentModel.DataAnnotations;

namespace clientIq_api.DTO
{
    public class ClientCreateDto
    {
        [Required]
        [StringLength(255)]
        public string CompanyName { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string ContactName { get; set; } = string.Empty;

        [EmailAddress]
        [StringLength(255)]
        public string? Email { get; set; }

        [StringLength(50)]
        public string? Phone { get; set; }

        public string? Address { get; set; }

        public string? Notes { get; set; }
    }

    public class ClientResponseDto
    {
        public int Id { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string ContactName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class ClientUpdateDto
    {
        [StringLength(255)]
        public string? CompanyName { get; set; }

        [StringLength(255)]
        public string? ContactName { get; set; }

        [EmailAddress]
        [StringLength(255)]
        public string? Email { get; set; }

        [StringLength(50)]
        public string? Phone { get; set; }

        public string? Address { get; set; }

        public string? Notes { get; set; }
    }
}