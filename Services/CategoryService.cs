// File: Services/CategoryService.cs
using Microsoft.EntityFrameworkCore; // For async DB operations
using MyntraClone.API.Data;           // For ApplicationDbContext
using MyntraClone.API.DTOs;           // For DTOs
using MyntraClone.API.Models;         // For Category model

namespace MyntraClone.API.Services
{
    public class CategoryService : ICategoryService // Implements the ICategoryService interface
    {
        private readonly ApplicationDbContext _context; // EF Core DbContext to interact with the database

        public CategoryService(ApplicationDbContext context)
        {
            _context = context; // Injecting the ApplicationDbContext
        }

        // Method to get all categories with optional name filtering and pagination
        public async Task<IEnumerable<CategoryDto>> GetAllAsync(string? name, int page, int pageSize)
        {
            var query = _context.Categories.AsQueryable(); // Start the query to fetch categories

            // If a name is provided, filter the categories by name (case-insensitive search)
            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(c => c.Name.Contains(name)); // Filter by name
            }

            // Fetch categories with pagination
            var categories = await query
                .Skip((page - 1) * pageSize)  // Skip to the correct page
                .Take(pageSize)               // Limit the number of categories to the page size
                .Select(c => new CategoryDto  // Project each category to a CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description
                })
                .ToListAsync(); // Execute the query and return the result as a list

            return categories; // Return the list of categories
        }

        // Method to get a category by its ID
        public async Task<CategoryDto?> GetByIdAsync(int id)
        {
            return await _context.Categories
                .Where(c => c.Id == id)
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description
                })
                .FirstOrDefaultAsync(); // Return the first matching category or null if not found
        }

        // Method to create a new category
        public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto)
        {
            var category = new Category
            {
                Name = dto.Name,
                Description = dto.Description,
            };

            _context.Categories.Add(category); // Add the new category to the DbContext
            await _context.SaveChangesAsync(); // Save the changes to the database

            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description
            }; // Return the newly created category as a DTO
        }

        // Method to update an existing category by ID
        public async Task<bool> UpdateAsync(int id, CreateCategoryDto dto)
        {
            var category = await _context.Categories.FindAsync(id); // Find the category by ID
            if (category == null)
                return false; // If category not found, return false

            category.Name = dto.Name;
            category.Description = dto.Description; // Update the category fields

            await _context.SaveChangesAsync(); // Save the changes to the database
            return true; // Return true to indicate the update was successful
        }

        // Method to delete a category by ID
        public async Task<bool> DeleteAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id); // Find the category by ID
            if (category == null)
                return false; // If category not found, return false

            _context.Categories.Remove(category); // Remove the category from the DbContext
            await _context.SaveChangesAsync(); // Save the changes to the database
            return true; // Return true to indicate the delete was successful
        }
    }
}
