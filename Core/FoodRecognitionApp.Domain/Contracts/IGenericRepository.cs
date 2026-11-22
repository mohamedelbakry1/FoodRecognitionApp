using FoodRecognitionApp.Domain.Entities;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Domain.Contracts
{
    public interface IGenericRepository<TKey,TEntity> where TEntity : BaseEntity<TKey>
    {
        Task<IEnumerable<TEntity>> GetAllAsync(ISpecification<TKey, TEntity> spec, bool changeTracker = false, CancellationToken ct = default);
        Task<TEntity?> GetById(ISpecification<TKey, TEntity> spec, bool changeTracker = false, CancellationToken ct = default);
        Task<int> CountAsync(ISpecification<TKey, TEntity> spec, CancellationToken ct = default);
        void Add(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);
    }
}
