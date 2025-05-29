using Microsoft.EntityFrameworkCore;
using MyntraClone.API.Data;
using MyntraClone.API.DTOs;
using MyntraClone.API.Models;

namespace MyntraClone.API.Services
{
    public class WishlistService : IWishlistService
    {
        private readonly ApplicationDbContext _db;

        public WishlistService(ApplicationDbContext db)
        {
            _db = db;
        }

        // 📦 Get the wishlist for a specific user
        public async Task<WishlistDto?> GetByUserIdAsync(int userId)
        {
            // 🔍 Fetch wishlist with items and product info
            var wishlist = await _db.Wishlists
                .Include(w => w.WishlistItems)
                .ThenInclude(wi => wi.Product)
                .FirstOrDefaultAsync(w => w.UserId == userId);

            if (wishlist == null) return null; // ❌ Wishlist doesn't exist

            // 🧱 Convert to DTO format
            return new WishlistDto
            {
                Id = wishlist.Id,
                UserId = wishlist.UserId,
                Items = wishlist.WishlistItems.Select(wi => new WishlistItemDto
                {
                    Id = wi.Id,
                    ProductId = wi.ProductId,
                    ProductName = wi.Product.Name,
                    Price = wi.Product.Price,
                    ImageUrl = wi.Product.ImageUrl
                }).ToList()
            };
        }

        // ➕ Add product to wishlist for the current user
        public async Task AddItemAsync(int userId, int productId, int quantity)
        {
            // 🔍 Get or create wishlist for user
            var wishlist = await _db.Wishlists
                .Include(w => w.WishlistItems)
                .FirstOrDefaultAsync(w => w.UserId == userId);

            if (wishlist == null)
            {
                // 🆕 If no wishlist exists, create one
                wishlist = new Wishlist { UserId = userId };
                _db.Wishlists.Add(wishlist);
                await _db.SaveChangesAsync(); // Save to generate WishlistId
            }

            // ✅ Check if product already exists in wishlist
            var alreadyExists = wishlist.WishlistItems
                .Any(wi => wi.ProductId == productId);

            if (!alreadyExists)
            {
                // ➕ Add new item to wishlist
                var item = new WishlistItem
                {
                    WishlistId = wishlist.Id,
                    ProductId = productId
                };
                _db.WishlistItems.Add(item);
                await _db.SaveChangesAsync();
            }
        }

        // ❌ Remove a wishlist item by its ID
        public async Task RemoveItemAsync(int itemId)
        {
            // 🔍 Look up item by ID
            var item = await _db.WishlistItems.FindAsync(itemId);

            if (item != null)
            {
                // 🗑️ Delete the item
                _db.WishlistItems.Remove(item);
                await _db.SaveChangesAsync();
            }
        }

        // 🔐 Find out which user owns a wishlist item
        public async Task<int?> GetUserIdByWishlistItemIdAsync(int itemId)
        {
            // 🔍 Join with wishlist to get userId
            return await _db.WishlistItems
                .Where(wi => wi.Id == itemId)
                .Include(wi => wi.Wishlist)
                .Select(wi => (int?)wi.Wishlist.UserId)
                .FirstOrDefaultAsync();
        }
    }
}
