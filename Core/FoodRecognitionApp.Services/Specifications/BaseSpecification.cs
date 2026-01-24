using FoodRecognitionApp.Domain.Contracts;
using FoodRecognitionApp.Domain.Entities;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace FoodRecognitionApp.Services.Specifications
{
    public abstract class BaseSpecification<TKEy, TEntity> : ISpecification<TKEy, TEntity> where TEntity : BaseEntity<TKEy>
    {
        public BaseSpecification(Expression<Func<TEntity,bool>> expression)
        {
            Criteria = expression;
        }
        public Expression<Func<TEntity, bool>> Criteria { get; set; }

        public List<Expression<Func<TEntity, object>>> Includes { get; set; } = new List<Expression<Func<TEntity, object>>>();

        public Expression<Func<TEntity, object>> OrderBy { get; set; }

        public Expression<Func<TEntity, object>> OrderByDescending { get; set; }

        public int Take { get; set;  }

        public int Skip { get; set; }

        public bool IsPagination { get; set; }

        public void ApplyPagination(int pageSize, int pageIndex)
        {
            Take = pageSize;
            IsPagination = true;
            Skip = pageSize * (pageIndex - 1);
        }

        public void AddOrderBy(Expression<Func<TEntity, object>> expression) => OrderBy = expression;

        public void AddOrderByDescending(Expression<Func<TEntity, object>> expression) => OrderByDescending = expression;
    }
}
