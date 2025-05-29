using System.ComponentModel.DataAnnotations;

namespace MyntraClone.API.DTOs
{
    public class CreateProductDto
    {
        [Required(ErrorMessage = "Product name is required.")]
        [StringLength(100)]
        public string Name { get; set; } = "";

        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; } = "";

        [Required(ErrorMessage = "Brand is required.")]
        public string Brand { get; set; } = "";

        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
        public decimal Price { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Discount price cannot be negative.")]
        public decimal DiscountPrice { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Stock must be a non-negative number.")]
        public int Stock { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Url(ErrorMessage = "Invalid image URL format.")]
        public string ImageUrl { get; set; } = "";
    }
}
