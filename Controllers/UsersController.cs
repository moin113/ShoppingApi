using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyntraClone.API.DTOs;
using MyntraClone.API.Services;

namespace MyntraClone.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]  // Routes like /api/users
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // ✅ Admin-only: Get all users
        [Authorize(Roles = "Admin")]  // Only Admins can access this action
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        // ✅ Authenticated user: Get a specific user by ID
        [HttpGet("{id}")]
        [Authorize] // Added explicit authorization to require authentication
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            return user == null ? NotFound() : Ok(user);
        }

        // ✅ Admin-only: Create a new user with model validation
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
        {
            // 🔐 Validate the input model
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var created = await _userService.CreateUserAsync(dto);
            return CreatedAtAction(nameof(GetUser), new { id = created.Id }, created);
        }

        // ✅ Authenticated user (or Admin): Update a user with validation
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto dto)
        {
            // 🔐 Validate the input model
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userIdStr = User.Identity?.Name;

            if (string.IsNullOrEmpty(userIdStr))
            {
                return Unauthorized("User is not logged in.");
            }

            // 🔐 Only allow the user to update their own info or if they are an Admin
            if (userIdStr != id.ToString() && !User.IsInRole("Admin"))
            {
                return Unauthorized("You are not allowed to update this user.");
            }

            var updated = await _userService.UpdateUserAsync(id, dto);
            return updated == null ? NotFound() : Ok(updated);
        }

        // ✅ Admin-only: Delete a user
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var result = await _userService.DeleteUserAsync(id);
            return result ? NoContent() : NotFound();
        }

        // ✅ Authenticated user: Get their own profile
        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            var userIdStr = User.Identity?.Name;

            if (string.IsNullOrEmpty(userIdStr))
            {
                return Unauthorized("User is not logged in.");
            }

            if (!int.TryParse(userIdStr, out var userId))
            {
                return BadRequest("User ID is not valid.");
            }

            var user = await _userService.GetUserByIdAsync(userId);
            return user == null ? NotFound() : Ok(user);
        }
    }
}