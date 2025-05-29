// File: Services/ICategoryService.cs
using MyntraClone.API.DTOs;

namespace MyntraClone.API.Services
{
    public interface ICategoryService
    {
        // Get all categories with optional name filter, pagination support (page, pageSize)
        Task<IEnumerable<CategoryDto>> GetAllAsync(string? name, int page, int pageSize); // Updated method signature with parameters for filtering and pagination

        Task<CategoryDto?> GetByIdAsync(int id); // Get category by id
        Task<CategoryDto> CreateAsync(CreateCategoryDto dto); // Create a new category
        Task<bool> UpdateAsync(int id, CreateCategoryDto dto); // Update an existing category
        Task<bool> DeleteAsync(int id); // Delete a category
    }
}
