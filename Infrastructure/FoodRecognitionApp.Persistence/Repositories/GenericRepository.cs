using FoodRecognitionApp.Domain.Contracts;
using FoodRecognitionApp.Domain.Entities;
using FoodRecognitionApp.Persistence.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Persistence.Repositories
{
    public class GenericRepository<TKey, TEntity>(AppDbContext _context) : IGenericRepository<TKey, TEntity> where TEntity : BaseEntity<TKey>
    {
        public async Task<IEnumerable<TEntity>> GetAllAsync(ISpecification<TKey, TEntity> spec, bool changeTracker = false)
        {
            return changeTracker ? await ApplySpecifications(spec).ToListAsync() : 
                                   await ApplySpecifications(spec).AsNoTracking().ToListAsync();
        }

        public async Task<TEntity?> GetById(ISpecification<TKey, TEntity> spec)
        {
            return await ApplySpecifications(spec).FirstOrDefaultAsync();
        }
        public async Task<int> CountAsync(ISpecification<TKey, TEntity> spec)
        {
            return await ApplySpecifications(spec).CountAsync();
        }

        public async Task AddAsync(TEntity entity)
        {
            await _context.Set<TEntity>().AddAsync(entity);
        }
        public void Update(TEntity entity)
        {
            _context.Set<TEntity>().Update(entity);
        }

        public void Delete(TEntity entity)
        {
            _context.Set<TEntity>().Remove(entity);
        }

        private IQueryable<TEntity> ApplySpecifications(ISpecification<TKey, TEntity> spec)
        {
            return SpecificationsEvaluator.GetQuery(_context.Set<TEntity>(), spec);
        }
    }
}
