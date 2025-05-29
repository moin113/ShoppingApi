using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyntraClone.API.Services;
using MyntraClone.API.DTOs; // DTOs for creating and returning product data
using MyntraClone.API.Services.FileService;


namespace MyntraClone.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _svc;

        // Constructor: Inject the ProductService for accessing product-related operations
        public ProductsController(IProductService svc)
        {
            _svc = svc;
        }

        // ✅ Public: Get all products with optional filtering and pagination
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? name,               // Optional filter by product name (partial match)
            [FromQuery] int? categoryId,            // Optional filter by category ID
            [FromQuery] int page = 1,               // Page number for pagination (default: 1)
            [FromQuery] int pageSize = 10)          // Page size for pagination (default: 10)
        {
            // Fetch filtered and paged product list from the service
            var products = await _svc.GetAllAsync(name, categoryId, page, pageSize);
            return Ok(products); // Return 200 OK with the list of products
        }

        // ✅ Public: Get a product by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            // Retrieve a single product by ID
            var product = await _svc.GetByIdAsync(id);
            // Return 404 Not Found if the product doesn't exist
            return product == null ? NotFound() : Ok(product);
        }

        // 🔐 Admin-only: Create a new product
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Post(CreateProductDto dto)
        {
            // Create the product using the service layer
            var createdProduct = await _svc.CreateAsync(dto);

            // Return 201 Created with location header pointing to the new resource
            return CreatedAtAction(nameof(Get), new { id = createdProduct.Id }, createdProduct);
        }

        // 🔐 Admin-only: Update an existing product by ID
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, CreateProductDto dto)
        {
            try
            {
                // Attempt to update the product
                await _svc.UpdateAsync(id, dto);
                // Return 204 No Content on success
                return NoContent();
            }
            catch (Exception)
            {
                // Return 404 Not Found if the product was not found
                return NotFound();
            }
        }
        [Authorize(Roles = "Admin")] // Optional: Only admins can upload
        [HttpPost("upload")]
        public async Task<IActionResult> UploadImage(IFormFile file, [FromServices] IFileService fileService)
        {
            try
            {
                var imageUrl = await fileService.SaveImageAsync(file);
                return Ok(new { Url = imageUrl });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }


        // 🔐 Admin-only: Delete a product by ID
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                // Attempt to delete the product
                await _svc.DeleteAsync(id);
                // Return 204 No Content on successful deletion
                return NoContent();
            }
            catch (Exception)
            {
                // Return 404 Not Found if the product was not found
                return NotFound();
            }
        }
    }
}
