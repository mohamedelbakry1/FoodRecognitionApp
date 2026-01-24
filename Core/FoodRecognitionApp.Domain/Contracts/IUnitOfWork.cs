using FoodRecognitionApp.Domain.Entities;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Domain.Contracts
{
    public interface IUnitOfWork
    {
        IGenericRepository<TKey, TEntity> GetRepository<TKey, TEntity>() where TEntity : BaseEntity<TKey>;

        Task<int> SaveChangesAsync();
    }
}
