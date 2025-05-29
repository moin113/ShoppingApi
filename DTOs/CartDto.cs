// File: DTOs/CartDto.cs
namespace MyntraClone.API.DTOs
{
    public class CartDto
    {
        public int Id { get; set; }          // Cart’s own ID
        public int UserId { get; set; }          // Owner of this cart
        public List<CartItemDto> Items { get; set; } = new(); // All line-items
    }
}
