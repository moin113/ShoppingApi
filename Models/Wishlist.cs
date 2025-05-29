namespace MyntraClone.API.Models;

public class Wishlist
{
    public int Id { get; set; }
    public int UserId { get; set; }

    public virtual User User { get; set; } = null!;
    public virtual ICollection<WishlistItem> WishlistItems { get; set; } = new List<WishlistItem>();
}
