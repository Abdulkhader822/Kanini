using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillAssessmentPortal.DTOs.Category;
using SkillAssessmentPortal.Services.Interfaces;

namespace SkillAssessmentPortal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(ICategoryService categoryService, ILogger<CategoryController> logger)
        {
            _categoryService = categoryService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("GET /api/Category - Fetching all categories");
            try
            {
                var categories = await _categoryService.GetAllAsync();
                _logger.LogInformation("Successfully retrieved {Count} categories", categories.Count());
                Response.Headers.Add("X-Status-Message", "Categories Retrieved Successfully");
                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all categories");
                return StatusCode(500, new { error = "Failed to retrieve categories from database. Please try again later." });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var category = await _categoryService.GetByIdAsync(id);
                if (category == null)
                    return BadRequest(new { error = $"Category with ID {id} not found." });

                return Ok(category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching category by ID: {Id}", id);
                return StatusCode(500, new { error = $"Failed to retrieve category with ID {id}. Please try again later." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CategoryCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return BadRequest(new { error = $"Invalid category data: {string.Join(", ", errors)}" });
            }

            try
            {
                var (isSuccess, message) = await _categoryService.CreateAsync(dto);
                if (!isSuccess)
                {
                    Response.StatusCode = 400;
                    Response.Headers.Add("X-Status-Message", "Category Creation Failed");
                    return BadRequest(new { error = message });
                }

                Response.Headers.Add("X-Status-Message", "Category Created Successfully");
                return Ok(new { message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating category");
                return StatusCode(500, new { error = "Failed to create category. Please check your data and try again." });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CategoryUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return BadRequest(new { error = $"Invalid category update data: {string.Join(", ", errors)}" });
            }

            try
            {
                var message = await _categoryService.UpdateAsync(id, dto);
                if (message.Contains("not found"))
                {
                    Response.StatusCode = 400;
                    Response.Headers.Add("X-Status-Message", "Category Update Failed - Not Found");
                    return BadRequest(new { error = $"Category with ID {id} not found for update." });
                }

                Response.Headers.Add("X-Status-Message", "Category Updated Successfully");
                return Ok(new { message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating category ID: {Id}", id);
                return StatusCode(500, new { error = $"Failed to update category with ID {id}. Please try again later." });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var message = await _categoryService.DeleteAsync(id);
                if (message.Contains("not found"))
                {
                    Response.StatusCode = 400;
                    Response.Headers.Add("X-Status-Message", "Category Deletion Failed - Not Found");
                    return BadRequest(new { error = $"Category with ID {id} not found for deletion." });
                }

                Response.Headers.Add("X-Status-Message", "Category Deleted Successfully");
                return Ok(new { message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting category ID: {Id}", id);
                return StatusCode(500, new { error = $"Failed to delete category with ID {id}. It may be in use by existing tests." });
            }
        }
    }
}
