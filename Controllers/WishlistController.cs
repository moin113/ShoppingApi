using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyntraClone.API.DTOs;
using MyntraClone.API.Services;
using System.Security.Claims;

namespace MyntraClone.API.Controllers;

[Authorize] // 🔐 Login required
[ApiController]
[Route("api/[controller]")]
public class WishlistController : ControllerBase
{
    private readonly IWishlistService _svc;

    public WishlistController(IWishlistService svc)
    {
        _svc = svc;
    }

    // ─── UPDATED: Get current user ID from claims ──────────────
    private int GetCurrentUserId()
    {
        // Primary approach: Use User.Identity.Name which is mapped to ClaimTypes.Name
        var name = User.Identity?.Name;
        if (!string.IsNullOrEmpty(name) && int.TryParse(name, out var id))
        {
            Console.WriteLine($"Found user ID from Identity.Name: {id}");
            return id;
        }

        // Fallback: Try NameIdentifier claim
        var nameIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!string.IsNullOrEmpty(nameIdClaim) && int.TryParse(nameIdClaim, out id))
        {
            Console.WriteLine($"Found user ID from NameIdentifier: {id}");
            return id;
        }

        // If all methods fail, log all claims for debugging
        Console.WriteLine("Failed to get user ID. Available claims:");
        foreach (var claim in User.Claims)
        {
            Console.WriteLine($"- {claim.Type}: {claim.Value}");
        }

        throw new UnauthorizedAccessException("User ID not found in token");
    }

    // ─── GET: Wishlist by userId ──────────────────────────────
    [HttpGet("{userId}")]
    public async Task<IActionResult> Get(int userId)
    {
        try
        {
            var currentUserId = GetCurrentUserId();

            // Security check: users can only view their own wishlists
            if (currentUserId != userId && !User.IsInRole("Admin"))
                return Forbid();      // 🚫 Accessing another user's wishlist

            var wishlist = await _svc.GetByUserIdAsync(userId);
            return wishlist == null ? NotFound() : Ok(wishlist);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized("Valid authentication token required");
        }
    }

    // ─── POST: Add item to wishlist ───────────────────────────
    [HttpPost("items")]
    public async Task<IActionResult> AddItem([FromBody] AddWishlistItemDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState); // 🚫 Validation failed

            var currentUserId = GetCurrentUserId();

            // ✅ Inject userId into service, don't trust client
            await _svc.AddItemAsync(currentUserId, dto.ProductId, dto.Quantity);

            return Ok();
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized("Valid authentication token required");
        }
    }

    // ─── DELETE: Remove item from wishlist ────────────────────
    [HttpDelete("items/{itemId}")]
    public async Task<IActionResult> RemoveItem(int itemId)
    {
        try
        {
            var currentUserId = GetCurrentUserId();

            var itemOwnerId = await _svc.GetUserIdByWishlistItemIdAsync(itemId);
            if (itemOwnerId == null) return NotFound();        // ❓ Wishlist item doesn't exist

            // Security check: users can only delete their own items
            if (itemOwnerId != currentUserId && !User.IsInRole("Admin"))
                return Forbid(); // 🚫 Not your item

            await _svc.RemoveItemAsync(itemId);                // ✅ Authorized
            return NoContent();
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized("Valid authentication token required");
        }
    }
}