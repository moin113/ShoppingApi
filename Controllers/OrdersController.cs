using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyntraClone.API.DTOs;
using MyntraClone.API.Services;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;

namespace MyntraClone.API.Controllers
{
    [Authorize]  // 🔐 All actions in this controller require authentication
    [ApiController]
    [Route("api/[controller]")]  // Route: /api/orders
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _svc;

        public OrdersController(IOrderService svc)
        {
            _svc = svc;
        }

        // 🔁 UPDATED: Helper method to extract logged-in user's ID from JWT
        // Prioritizing User.Identity.Name which is mapped to ClaimTypes.Name
        private int GetCurrentUserId()
        {
            // Primary approach: Use User.Identity.Name which is mapped to ClaimTypes.Name
            var name = User.Identity?.Name;
            if (!string.IsNullOrEmpty(name) && int.TryParse(name, out var id))
            {
                Console.WriteLine($"Found user ID from Identity.Name: {id}");
                return id;
            }

            // Fallback: Check the NameIdentifier claim
            var nameIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(nameIdClaim) && int.TryParse(nameIdClaim, out id))
            {
                Console.WriteLine($"Found user ID from NameIdentifier: {id}");
                return id;
            }

            // Log all available claims for debugging
            Console.WriteLine("Available claims:");
            foreach (var claim in User.Claims)
            {
                Console.WriteLine($"{claim.Type} = {claim.Value}");
            }

            // If we reach here, no valid user ID was found in claims
            throw new UnauthorizedAccessException("No valid user ID found in token");
        }

        // ✅ POST: /api/orders
        // Place a new order (validated DTO)
        [HttpPost]
        public async Task<IActionResult> PlaceOrder([FromBody] CreateOrderDto dto)
        {
            try
            {
                // Check if incoming DTO is valid (Model validation)
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Get current user ID and verify it matches the DTO
                int currentUserId = GetCurrentUserId();
                if (currentUserId != dto.UserId)
                {
                    // Security check: prevent ordering for other users
                    return Forbid("Cannot place orders for other users");
                }

                var order = await _svc.PlaceOrderAsync(dto);
                return CreatedAtAction(nameof(GetById), new { orderId = order.Id }, order);
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

        // ✅ GET: /api/orders/user/{userId}
        // Only allow users to see *their own* orders
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserOrders(int userId)
        {
            try
            {
                var currentUserId = GetCurrentUserId();

                // Only allow users to see their own orders (unless admin)
                if (currentUserId != userId && !User.IsInRole("Admin"))
                    return Forbid();  // 🚫 Prevent viewing other users' orders

                var list = await _svc.GetOrdersByUserAsync(userId);
                return Ok(list);
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

        // ✅ GET: /api/orders
        // Only Admins can fetch *all* orders in the system
        [HttpGet]
        [Authorize(Roles = "Admin")]  // 🔐 Admin-only access
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var all = await _svc.GetAllOrdersAsync();
                return Ok(all);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        // ✅ GET: /api/orders/{orderId}
        // Anyone logged in can fetch their own order by ID
        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetById(int orderId)
        {
            try
            {
                var order = await _svc.GetOrderByIdAsync(orderId);
                if (order == null)
                    return NotFound();

                var currentUserId = GetCurrentUserId();

                // Allow viewing only if it's the user's own order or user is admin
                if (order.UserId != currentUserId && !User.IsInRole("Admin"))
                    return Forbid();  // 🚫 Block access to others' orders

                return Ok(order);
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