// File: DTOs/CategoryDto.cs
namespace MyntraClone.API.DTOs
{
    // This DTO is returned when fetching category details
    public class CategoryDto
    {
        public int Id { get; set; }           // Unique identifier for the category
        public string Name { get; set; } = ""; // Name of the category
        public string Description { get; set; } = ""; // Optional but useful
    }
}
