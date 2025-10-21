using System.ComponentModel.DataAnnotations;

namespace clientIq_api.DTO
{
    public class ProductCreateDto
    {
        [Required]
        [StringLength(255)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        [Range(0.01, 999999.99)]
        public decimal Price { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class ProductResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class ProductUpdateDto
    {
        [StringLength(255)]
        public string? Name { get; set; }

        public string? Description { get; set; }

        [Range(0.01, 999999.99)]
        public decimal? Price { get; set; }

        public bool? IsActive { get; set; }
    }
}