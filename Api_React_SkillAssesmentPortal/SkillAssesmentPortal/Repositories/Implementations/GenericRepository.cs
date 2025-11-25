using Microsoft.EntityFrameworkCore;
using SkillAssessmentPortal.Models;
using SkillAssessmentPortal.Repositories.Interfaces;

namespace SkillAssessmentPortal.Repositories.Implementations
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly SkillAssessmentDbContext _context;
        private readonly DbSet<T> _dbSet;
        protected readonly ILogger<GenericRepository<T>> _logger;

        public GenericRepository(SkillAssessmentDbContext context, ILogger<GenericRepository<T>> logger)
        {
            _context = context;
            _dbSet = _context.Set<T>();
            _logger = logger;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            _logger.LogInformation("Fetching all {EntityType} entities", typeof(T).Name);
            try
            {
                var entities = await _dbSet.ToListAsync();
                _logger.LogInformation("Successfully retrieved {Count} {EntityType} entities", entities.Count, typeof(T).Name);
                return entities;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all {EntityType} entities", typeof(T).Name);
                throw;
            }
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            _logger.LogInformation("Fetching {EntityType} entity by ID: {Id}", typeof(T).Name, id);
            try
            {
                var entity = await _dbSet.FindAsync(id);
                if (entity != null)
                    _logger.LogInformation("{EntityType} entity found for ID: {Id}", typeof(T).Name, id);
                else
                    _logger.LogWarning("{EntityType} entity not found for ID: {Id}", typeof(T).Name, id);
                return entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching {EntityType} entity by ID: {Id}", typeof(T).Name, id);
                throw;
            }
        }

        public async Task AddAsync(T entity)
        {
            _logger.LogInformation("Adding new {EntityType} entity", typeof(T).Name);
            try
            {
                await _dbSet.AddAsync(entity);
                await _context.SaveChangesAsync();
                _logger.LogInformation("{EntityType} entity added successfully", typeof(T).Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding {EntityType} entity", typeof(T).Name);
                throw;
            }
        }

        public async Task UpdateAsync(T entity)
        {
            _logger.LogInformation("Updating {EntityType} entity", typeof(T).Name);
            try
            {
                _dbSet.Update(entity);
                await _context.SaveChangesAsync();
                _logger.LogInformation("{EntityType} entity updated successfully", typeof(T).Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating {EntityType} entity", typeof(T).Name);
                throw;
            }
        }

        public async Task DeleteAsync(int id)
        {
            _logger.LogInformation("Deleting {EntityType} entity with ID: {Id}", typeof(T).Name, id);
            try
            {
                var entity = await GetByIdAsync(id);
                if (entity != null)
                {
                    _dbSet.Remove(entity);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("{EntityType} entity deleted successfully for ID: {Id}", typeof(T).Name, id);
                }
                else
                {
                    _logger.LogWarning("{EntityType} entity not found for deletion, ID: {Id}", typeof(T).Name, id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting {EntityType} entity with ID: {Id}", typeof(T).Name, id);
                throw;
            }
        }
    }
}
