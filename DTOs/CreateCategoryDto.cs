using System.ComponentModel.DataAnnotations;

namespace MyntraClone.API.DTOs
{
    // This DTO is used when creating a new category via POST request
    public class CreateCategoryDto
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
        public string Name { get; set; } = "";

        [Required(ErrorMessage = "Description is required")]
        [StringLength(300, ErrorMessage = "Description cannot exceed 300 characters")]
        public string Description { get; set; } = "";
    }
}
