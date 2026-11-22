using FoodRecognitionApp.Domain.Contracts;
using FoodRecognitionApp.Domain.Entities;
using FoodRecognitionApp.Persistence.Data.Contexts;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Persistence.Repositories
{
    internal class UnitOfWork(AppDbContext _context) : IUnitOfWork
    {
        private readonly ConcurrentDictionary<string, object> _repositories = new ConcurrentDictionary<string, object>();

        public IGenericRepository<TKey, TEntity> GetRepository<TKey, TEntity>() where TEntity : BaseEntity<TKey>
        {
            return (IGenericRepository<TKey,TEntity>) _repositories.GetOrAdd(typeof(TEntity).Name, new GenericRepository<TKey,TEntity>(_context));
        }

        public async Task<int> SaveChangesAsync(CancellationToken ct = default)
        {
            return await _context.SaveChangesAsync(ct);
        }
    }
}
