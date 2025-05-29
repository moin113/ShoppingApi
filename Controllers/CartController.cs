using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using MyntraClone.API.DTOs;
using MyntraClone.API.Services;

namespace MyntraClone.API.Controllers
{
    // ✅ Restricts this controller to only users with the "Customer" role
    [Authorize(Roles = "Customer")]
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _svc;

        public CartController(ICartService svc)
        {
            _svc = svc;
        }

        // ✅ UPDATED: Simplified user ID extraction focusing on ClaimTypes.Name
        private int GetUserId()
        {
            // Primary approach: Use User.Identity.Name which is mapped to ClaimTypes.Name
            var name = User.Identity?.Name;
            if (!string.IsNullOrEmpty(name) && int.TryParse(name, out var userId))
            {
                Console.WriteLine($"Using Identity.Name for userId: {userId}");
                return userId;
            }

            // Fallback: Try NameIdentifier claim
            var nameIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (nameIdClaim != null && int.TryParse(nameIdClaim.Value, out userId))
            {
                Console.WriteLine($"Found NameIdentifier claim: {nameIdClaim.Value}");
                return userId;
            }

            // Last resort: JWT standard 'sub' claim
            var subClaim = User.FindFirst("sub");
            if (subClaim != null && int.TryParse(subClaim.Value, out userId))
            {
                Console.WriteLine($"Using sub claim for userId: {subClaim.Value}");
                return userId;
            }

            // Log all available claims for debugging
            Console.WriteLine("Available claims:");
            foreach (var claim in User.Claims)
            {
                Console.WriteLine($"{claim.Type} = {claim.Value}");
            }

            throw new UnauthorizedAccessException("User ID not found in token claims.");
        }

        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            try
            {
                var userId = GetUserId();
                var cart = await _svc.GetCartAsync(userId);
                return Ok(cart);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddItem([FromBody] CreateCartItemDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var userId = GetUserId();
                var updatedCart = await _svc.AddItemAsync(userId, dto);
                return Ok(updatedCart);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpDelete("items/{itemId}")]
        public async Task<IActionResult> RemoveItem(int itemId)
        {
            try
            {
                var userId = GetUserId();
                var success = await _svc.RemoveItemAsync(userId, itemId);
                return success ? NoContent() : NotFound();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpDelete("clear")]
        public async Task<IActionResult> ClearCart()
        {
            try
            {
                var userId = GetUserId();
                var success = await _svc.ClearCartAsync(userId);
                return success ? NoContent() : NotFound();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }
    }
}