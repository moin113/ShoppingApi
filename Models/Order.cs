namespace MyntraClone.API.Models;

public class Order
{
    public int Id { get; set; }
    public int UserId { get; set; }

    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = "Pending";

    public DateTime OrderDate { get; set; } = DateTime.Now;

    public required string ShippingAddress { get; set; } = "";
    public required string PaymentIntentId { get; set; } = "";

    public string PaymentReference { get; set; } = ""; // ✅ Added

    public virtual User User { get; set; } = null!;
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
