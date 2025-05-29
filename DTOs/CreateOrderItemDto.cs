using System.ComponentModel.DataAnnotations;

namespace MyntraClone.API.DTOs
{
    public class CreateOrderItemDto
    {
        [Required(ErrorMessage = "ProductId is required")]
        public int ProductId { get; set; }   // ID of the product being ordered

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }    // Units of that product

        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }   // Price at time of order (to preserve history)
    }
}
