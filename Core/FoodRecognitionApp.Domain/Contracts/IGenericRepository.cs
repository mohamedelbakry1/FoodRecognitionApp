using FoodRecognitionApp.Domain.Entities;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Domain.Contracts
{
    public interface IGenericRepository<TKey,TEntity> where TEntity : BaseEntity<TKey>
    {
        Task<IEnumerable<TEntity>> GetAllAsync(ISpecification<TKey, TEntity> spec, bool changeTracker = false);
        Task<TEntity?> GetById(ISpecification<TKey, TEntity> spec);
        Task<int> CountAsync(ISpecification<TKey, TEntity> spec);
        Task AddAsync(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);
    }
}
