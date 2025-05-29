namespace MyntraClone.API.DTOs;

public class WishlistItemDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = "";
    public decimal Price { get; set; }
    public string ImageUrl { get; set; } = "";
}
