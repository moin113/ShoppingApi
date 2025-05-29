namespace MyntraClone.API.Models;

public class Product
{
    public int Id { get; set; }

    public required string Name { get; set; } = "";
    public required string Description { get; set; } = "";
    public required string Brand { get; set; } = "";
    public required string ImageUrl { get; set; } = "";

    public decimal Price { get; set; }
    public decimal DiscountPrice { get; set; }
    public int Stock { get; set; }
    public int CategoryId { get; set; }

    public virtual Category Category { get; set; } = null!;
    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    public virtual ICollection<WishlistItem> WishlistItems { get; set; } = new List<WishlistItem>();
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
