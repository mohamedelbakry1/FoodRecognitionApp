using FoodRecognitionApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace FoodRecognitionApp.Domain.Contracts
{
    public interface ISpecification<TKey,TEntity> where TEntity : BaseEntity<TKey>
    {
        Expression<Func<TEntity, bool>> Criteria { get; }
        List<Expression<Func<TEntity,object>>> Includes { get; }
        Expression<Func<TEntity,object>> OrderBy { get; }
        Expression<Func<TEntity,object>> OrderByDescending { get; }
        int Take { get; }
        int Skip { get; }
        bool IsPagination { get; }
    }
}
