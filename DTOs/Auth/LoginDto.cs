using System.ComponentModel.DataAnnotations;  // ✅ Required for validation attributes

namespace MyntraClone.API.DTOs.Auth
{
    // ✅ DTO for user login input
    public class LoginDto
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; } = "";
    }
}