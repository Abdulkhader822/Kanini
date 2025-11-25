using Microsoft.EntityFrameworkCore;
using SkillAssessmentPortal.Models;
using SkillAssessmentPortal.Repositories.Interfaces;

namespace SkillAssessmentPortal.Repositories.Implementations
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly SkillAssessmentDbContext _context;
        private readonly ILogger<CategoryRepository> _logger;

        public CategoryRepository(SkillAssessmentDbContext context, ILogger<CategoryRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            _logger.LogInformation("Fetching all categories from database");
            try
            {
                var categories = await _context.Categories.AsNoTracking().ToListAsync();
                _logger.LogInformation("Successfully retrieved {Count} categories", categories.Count);
                return categories;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all categories");
                throw;
            }
        }

        public async Task<Category?> GetByIdAsync(int id)
        {
            _logger.LogInformation("Fetching category by ID: {CategoryId}", id);
            try
            {
                var category = await _context.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.CategoryId == id);
                if (category != null)
                    _logger.LogInformation("Category found for ID: {CategoryId}", id);
                else
                    _logger.LogWarning("Category not found for ID: {CategoryId}", id);
                return category;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching category by ID: {CategoryId}", id);
                throw;
            }
        }

        public async Task<Category?> GetByNameAsync(string categoryName)
        {
            _logger.LogInformation("Fetching category by name: {CategoryName}", categoryName);
            try
            {
                var category = await _context.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.CategoryName == categoryName);
                if (category != null)
                    _logger.LogInformation("Category found for name: {CategoryName}", categoryName);
                else
                    _logger.LogInformation("Category not found for name: {CategoryName}", categoryName);
                return category;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching category by name: {CategoryName}", categoryName);
                throw;
            }
        }

        public async Task<Category> AddAsync(Category category)
        {
            _logger.LogInformation("Adding new category: {CategoryName}", category.CategoryName);
            try
            {
                _context.Categories.Add(category);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Category added successfully with ID: {CategoryId}", category.CategoryId);
                return category;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding category: {CategoryName}", category.CategoryName);
                throw;
            }
        }

        public async Task UpdateAsync(Category category)
        {
            _logger.LogInformation("Updating category ID: {CategoryId}", category.CategoryId);
            try
            {
                _context.Categories.Update(category);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Category updated successfully for ID: {CategoryId}", category.CategoryId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating category ID: {CategoryId}", category.CategoryId);
                throw;
            }
        }

        public async Task DeleteAsync(int id)
        {
            _logger.LogInformation("Deleting category ID: {CategoryId}", id);
            try
            {
                var category = await _context.Categories.FindAsync(id);
                if (category != null)
                {
                    _context.Categories.Remove(category);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Category deleted successfully for ID: {CategoryId}", id);
                }
                else
                {
                    _logger.LogWarning("Category not found for deletion, ID: {CategoryId}", id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting category ID: {CategoryId}", id);
                throw;
            }
        }
    }
}
