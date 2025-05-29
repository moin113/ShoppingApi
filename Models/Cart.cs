namespace MyntraClone.API.Models;

public class Cart
{
    public int Id { get; set; }
    public int UserId { get; set; }

    public virtual User User { get; set; } = null!;
    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
}
