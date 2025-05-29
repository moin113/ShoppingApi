namespace MyntraClone.API.DTOs;

public class WishlistDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public List<WishlistItemDto> Items { get; set; } = new();
}
