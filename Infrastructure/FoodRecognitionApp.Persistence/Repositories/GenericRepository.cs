using FoodRecognitionApp.Domain.Contracts;
using FoodRecognitionApp.Domain.Entities;
using FoodRecognitionApp.Persistence.Data.Contexts;
using FoodRecognitionApp.Persistence.Sepecifications;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Persistence.Repositories
{
    internal class GenericRepository<TKey, TEntity>(AppDbContext _context) : IGenericRepository<TKey, TEntity> where TEntity : BaseEntity<TKey>
    {
        public async Task<IEnumerable<TEntity>> GetAllAsync(ISpecification<TKey, TEntity> spec, bool changeTracker = false, CancellationToken ct = default)
        {
            return changeTracker ? await ApplySpecifications(spec).ToListAsync(ct) : 
                                   await ApplySpecifications(spec).AsNoTracking().ToListAsync(ct);
        }

        public async Task<TEntity?> GetById(ISpecification<TKey, TEntity> spec, bool changeTracker = false, CancellationToken ct = default)
        {
            return changeTracker ? await ApplySpecifications(spec).FirstOrDefaultAsync(ct) : 
                                   await ApplySpecifications(spec).AsNoTracking().FirstOrDefaultAsync(ct);
        }
        public async Task<int> CountAsync(ISpecification<TKey, TEntity> spec, CancellationToken ct = default)
        {
            return await ApplySpecifications(spec).CountAsync(ct);
        }

        public void Add(TEntity entity)
        {
            _context.Set<TEntity>().Add(entity);
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
