using System.ComponentModel.DataAnnotations;

namespace MyntraClone.API.DTOs
{
    public class CreateOrderDto
    {
        [Required(ErrorMessage = "UserId is required")]
        public int UserId { get; set; }                   // Who is placing the order

        [Required(ErrorMessage = "Order items are required")]
        [MinLength(1, ErrorMessage = "At least one item is required in the order")]
        public List<CreateOrderItemDto> Items { get; set; } = new();

        [Required(ErrorMessage = "Shipping address is required")]
        [MinLength(5, ErrorMessage = "Shipping address must be at least 5 characters long")]
        public string ShippingAddress { get; set; } = ""; // Where to ship

        [Required(ErrorMessage = "Payment reference is required")]
        public string PaymentReference { get; set; } = ""; // From Stripe

        [Required(ErrorMessage = "Payment Intent ID is required")]
        public string PaymentIntentId { get; set; } = ""; // From Stripe
    }
}
