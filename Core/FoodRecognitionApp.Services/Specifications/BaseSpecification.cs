using FoodRecognitionApp.Domain.Contracts;
using FoodRecognitionApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace FoodRecognitionApp.Services.Specifications
{
    internal abstract class BaseSpecification<TKEy, TEntity> : ISpecification<TKEy, TEntity> where TEntity : BaseEntity<TKEy>
    {
        public BaseSpecification(Expression<Func<TEntity,bool>> expression)
        {
            Criteria = expression;
        }
        public ICollection<Expression<Func<TEntity, object>>> Includes { get; } = [];

        public Expression<Func<TEntity, bool>> Criteria { get; private set; }

        public Expression<Func<TEntity, object>>? OrderBy { get; private set; }

        public Expression<Func<TEntity, object>>? OrderByDescending { get; private set; }

        public int Take { get; private set;  }

        public int Skip { get; private set; }

        public bool IsPaginated { get; private set; }

        public void ApplyPagination(int pageSize, int pageIndex)
        {
            Take = pageSize;
            IsPaginated = true;
            Skip = pageSize * (pageIndex - 1);
        }

        public void AddOrderBy(Expression<Func<TEntity, object>> expression) => OrderBy = expression;

        public void AddOrderByDescending(Expression<Func<TEntity, object>> expression) => OrderByDescending = expression;
    }
}
