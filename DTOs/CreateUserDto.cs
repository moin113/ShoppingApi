using System.ComponentModel.DataAnnotations;

namespace MyntraClone.API.DTOs
{
    public class CreateUserDto
    {
        [Required(ErrorMessage = "Name zaruri hai.")]
        [MinLength(2, ErrorMessage = "Name kam se kam 2 characters ka hona chahiye.")]
        public string Name { get; set; } = "";

        [Required(ErrorMessage = "Email zaruri hai.")]
        [EmailAddress(ErrorMessage = "Sahi email format dijiye.")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Password zaruri hai.")]
        [MinLength(6, ErrorMessage = "Password kam se kam 6 characters ka hona chahiye.")]
        public string Password { get; set; } = "";
    }
}
