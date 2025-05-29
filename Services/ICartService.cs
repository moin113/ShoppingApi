// File: Services/ICartService.cs
using MyntraClone.API.DTOs;

namespace MyntraClone.API.Services
{
    public interface ICartService
    {
        Task<CartDto> GetCartAsync(int userId);                             // Fetch the user's cart
        Task<CartDto> AddItemAsync(int userId, CreateCartItemDto dto);     // Add or update a cart item
        Task<bool> RemoveItemAsync(int userId, int cartItemId);            // Remove one item
        Task<bool> ClearCartAsync(int userId);                             // Remove all items
    }
}
