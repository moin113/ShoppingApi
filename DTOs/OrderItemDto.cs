// File: DTOs/OrderItemDto.cs
namespace MyntraClone.API.DTOs
{
    public class OrderItemDto
    {
        public int Id { get; set; } // OrderItem’s own ID
        public int ProductId { get; set; }
        public string ProductName { get; set; } = "";
        public int Quantity { get; set; }
        public decimal Price { get; set; } // Unit price charged
    }
}
