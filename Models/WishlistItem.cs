namespace MyntraClone.API.Models;

public class WishlistItem
{
    public int Id { get; set; }
    public int WishlistId { get; set; }
    public int ProductId { get; set; }

    public virtual Wishlist Wishlist { get; set; } = null!;
    public virtual Product Product { get; set; } = null!;
}
