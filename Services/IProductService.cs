using MyntraClone.API.DTOs;

public interface IProductService
{
    Task<List<ProductDto>> GetAllAsync(string? name, int? categoryId, int page, int pageSize);
    Task<ProductDto?> GetByIdAsync(int id);
    Task<ProductDto> CreateAsync(CreateProductDto dto);  // 🔁 Return created product
    Task<bool> UpdateAsync(int id, CreateProductDto dto); // ✅ Return success/failure
    Task<bool> DeleteAsync(int id);                      // ✅ Return success/failure
}
