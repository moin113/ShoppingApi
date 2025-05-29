using System.ComponentModel.DataAnnotations;

namespace MyntraClone.API.DTOs
{
    public class UpdateUserDto
    {
        [Required(ErrorMessage = "Name zaruri hai.")]
        [MinLength(2, ErrorMessage = "Name kam se kam 2 characters ka hona chahiye.")]
        public string Name { get; set; } = "";

        [Required(ErrorMessage = "Email zaruri hai.")]
        [EmailAddress(ErrorMessage = "Sahi email format dijiye.")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Role zaruri hai.")]
        [RegularExpression("Admin|Customer", ErrorMessage = "Role sirf 'Admin' ya 'Customer' ho sakta hai.")]
        public string Role { get; set; } = "Customer";
    }
}
