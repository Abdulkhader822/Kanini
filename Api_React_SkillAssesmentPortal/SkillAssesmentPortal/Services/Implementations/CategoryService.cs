using Microsoft.Extensions.Caching.Memory;
using SkillAssessmentPortal.DTOs.Category;
using SkillAssessmentPortal.Models;
using SkillAssessmentPortal.Repositories.Interfaces;
using SkillAssessmentPortal.Services.Interfaces;

namespace SkillAssessmentPortal.Services.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMemoryCache _cache;
        private readonly ILogger<CategoryService> _logger;
        private const string CACHE_KEY = "categories_all";
        private readonly TimeSpan CACHE_DURATION = TimeSpan.FromMinutes(30);

        public CategoryService(ICategoryRepository categoryRepository, IMemoryCache cache, ILogger<CategoryService> logger)
        {
            _categoryRepository = categoryRepository;
            _cache = cache;
            _logger = logger;
        }

        public async Task<IEnumerable<CategoryResponseDto>> GetAllAsync()
        {
            if (_cache.TryGetValue(CACHE_KEY, out IEnumerable<CategoryResponseDto>? cached))
                return cached!;

            var categories = await _categoryRepository.GetAllAsync();
            var result = categories.Select(c => new CategoryResponseDto
            {
                CategoryId = c.CategoryId,
                CategoryName = c.CategoryName,
                Description = c.Description
            }).ToList();
            
            _cache.Set(CACHE_KEY, result, CACHE_DURATION);
            return result;
        }

        public async Task<CategoryResponseDto?> GetByIdAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null) return null;

            return new CategoryResponseDto
            {
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName,
                Description = category.Description
            };
        }

        public async Task<(bool IsSuccess, string Message)> CreateAsync(CategoryCreateDto dto)
        {
            _logger.LogInformation("Creating new category: {CategoryName}", dto.CategoryName);
            
            var existing = await _categoryRepository.GetByNameAsync(dto.CategoryName);
            if (existing != null)
            {
                _logger.LogWarning("Category creation failed - Category already exists: {CategoryName}", dto.CategoryName);
                return (false, "Category already exists.");
            }

            var newCategory = new Category
            {
                CategoryName = dto.CategoryName,
                Description = dto.Description
            };

            await _categoryRepository.AddAsync(newCategory);
            _cache.Remove(CACHE_KEY);
            return (true, "Category created successfully.");
        }

        public async Task<string> UpdateAsync(int id, CategoryUpdateDto dto)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null) return "Category not found.";

            category.CategoryName = dto.CategoryName;
            category.Description = dto.Description;

            await _categoryRepository.UpdateAsync(category);
            _cache.Remove(CACHE_KEY);
            return "Category updated successfully.";
        }

        public async Task<string> DeleteAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null) return "Category not found.";

            await _categoryRepository.DeleteAsync(id);
            _cache.Remove(CACHE_KEY);
            return "Category deleted successfully.";
        }
    }
}
