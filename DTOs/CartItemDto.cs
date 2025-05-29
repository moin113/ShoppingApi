// File: DTOs/CartItemDto.cs
namespace MyntraClone.API.DTOs
{
    public class CartItemDto
    {
        public int Id { get; set; }   // CartItem’s own ID
        public int ProductId { get; set; }   // ID of the product
        public string ProductName { get; set; } = ""; // For display in UI
        public int Quantity { get; set; }   // How many units
        public decimal Price { get; set; }   // Unit price (so frontend can calculate totals)
    }
}
