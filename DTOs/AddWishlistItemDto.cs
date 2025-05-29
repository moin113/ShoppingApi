using System.ComponentModel.DataAnnotations;

namespace MyntraClone.API.DTOs;

public class AddWishlistItemDto
{
    [Required(ErrorMessage = "ProductId is required")]
    public int ProductId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
    public int Quantity { get; set; } = 1; // Optional: default to 1
}
