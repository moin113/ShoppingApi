namespace MyntraClone.API.DTOs
{
    public class OrderDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime OrderDate { get; set; }      // ✅ Corrected name
        public string ShippingAddress { get; set; } = "";
        public string PaymentReference { get; set; } = "";   // ✅ Now included
        public List<OrderItemDto> OrderItems { get; set; } = new();  // ✅ Corrected name
        public decimal TotalAmount { get; set; }
    }
}
