using Microsoft.EntityFrameworkCore;
using MyntraClone.API.Data;
using MyntraClone.API.DTOs;
using MyntraClone.API.Models;

namespace MyntraClone.API.Services
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _db;

        // Constructor that ensures the _db field is initialized with the provided ApplicationDbContext
        public ProductService(ApplicationDbContext db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        // ✅ Get all products with optional filtering by name, categoryId and pagination
        public async Task<List<ProductDto>> GetAllAsync(string? name, int? categoryId, int page, int pageSize)
        {
            var query = _db.Products
                .Include(p => p.Category)  // Include the category details for each product
                .AsQueryable();

            // Optional filtering by name (case-insensitive)
            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(p => p.Name.Contains(name)); // Filter products by name if provided
            }

            // Optional filtering by categoryId
            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId); // Filter products by category if provided
            }

            // Pagination (skip and take logic)
            var products = await query
                .OrderBy(p => p.Id)  // Optional: You can change the order if needed
                .Skip((page - 1) * pageSize)  // Skip products based on page number and pageSize
                .Take(pageSize)  // Take the number of products per page
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    CategoryName = p.Category.Name  // Get category name from related category entity
                })
                .ToListAsync();  // Asynchronously execute the query and return the list of products

            return products;
        }

        // ✅ Get a specific product by its ID
        public async Task<ProductDto?> GetByIdAsync(int id)
        {
            var p = await _db.Products
                             .Include(p => p.Category)  // Include category details
                             .FirstOrDefaultAsync(p => p.Id == id);  // Find the product by its ID

            if (p == null) return null;  // Return null if product not found

            // Map the found product to a ProductDto for a simplified response
            return new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                CategoryName = p.Category.Name  // Get category name from related category entity
            };
        }

        // ✅ Create a new product
        public async Task<ProductDto> CreateAsync(CreateProductDto dto)
        {
            // Map CreateProductDto to Product entity to insert into the database
            var entity = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Brand = dto.Brand,
                Price = dto.Price,
                DiscountPrice = dto.DiscountPrice,
                Stock = dto.Stock,
                CategoryId = dto.CategoryId,
                ImageUrl = dto.ImageUrl  // Set the image URL for the new product
            };

            _db.Products.Add(entity);  // Add the new product entity to the database
            await _db.SaveChangesAsync();  // Save the changes to the database

            // Load category name
            var categoryName = (await _db.Categories.FindAsync(dto.CategoryId))?.Name ?? "";

            // Return a ProductDto representing the newly created product
            return new ProductDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Price = entity.Price,
                CategoryName = categoryName
            };
        }

        // ✅ Update an existing product
        public async Task<bool> UpdateAsync(int id, CreateProductDto dto)
        {
            // Find the product by ID
            var product = await _db.Products.FindAsync(id);
            if (product == null) return false;  // Return false if product not found

            // Update the product details from the CreateProductDto
            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Brand = dto.Brand;
            product.Price = dto.Price;
            product.DiscountPrice = dto.DiscountPrice;
            product.Stock = dto.Stock;
            product.ImageUrl = dto.ImageUrl;
            product.CategoryId = dto.CategoryId;

            await _db.SaveChangesAsync();  // Save the updated product back to the database
            return true;  // Return true to indicate that the product was successfully updated
        }

        // ✅ Delete an existing product
        public async Task<bool> DeleteAsync(int id)
        {
            // Find the product by ID
            var product = await _db.Products.FindAsync(id);
            if (product == null) return false;  // Return false if product not found

            _db.Products.Remove(product);  // Remove the product from the database
            await _db.SaveChangesAsync();  // Save changes to the database

            return true;  // Return true to indicate that the product was successfully deleted
        }
    }
}
