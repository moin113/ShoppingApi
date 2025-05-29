using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyntraClone.API.DTOs;
using MyntraClone.API.Services;

namespace MyntraClone.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _service;

        public CategoriesController(ICategoryService service)
        {
            _service = service;
        }

        // ✅ Public: Get all categories with optional filtering and pagination
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? name, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var categories = await _service.GetAllAsync(name, page, pageSize);
            return Ok(categories);
        }

        // ✅ Public: Get category by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await _service.GetByIdAsync(id);
            return category == null ? NotFound() : Ok(category);
        }

        // ✅ Admin-only: Create a new category
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCategoryDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState); // ✅ Explicit validation

            var created = await _service.CreateAsync(dto); // ✅ Correct method
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // ✅ Admin-only: Update a category
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateCategoryDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var success = await _service.UpdateAsync(id, dto);
            return success ? NoContent() : NotFound();
        }

        // ✅ Admin-only: Delete a category
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _service.DeleteAsync(id);
            return success ? NoContent() : NotFound();
        }
    }
}
