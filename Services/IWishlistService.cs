using MyntraClone.API.DTOs;

namespace MyntraClone.API.Services
{
    public interface IWishlistService
    {
        // 🛒 Fetch wishlist by user ID
        Task<WishlistDto?> GetByUserIdAsync(int userId);

        // ➕ Add item securely (userId comes from token)
        Task AddItemAsync(int userId, int productId, int quantity);

        // ❌ Remove item from the wishlist
        Task RemoveItemAsync(int itemId);

        // 🔐 Get user ID by wishlist item ID
        Task<int?> GetUserIdByWishlistItemIdAsync(int itemId);
    }
}
