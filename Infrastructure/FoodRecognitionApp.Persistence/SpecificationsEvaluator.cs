using FoodRecognitionApp.Domain.Contracts;
using FoodRecognitionApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Persistence
{
    public static class SpecificationsEvaluator
    {
        public static IQueryable<TEntity> GetQuery<TKey,TEntity>(IQueryable<TEntity> inputQuery, ISpecification<TKey,TEntity> specification) where TEntity : BaseEntity<TKey>
        {
            var query = inputQuery;

            if(specification.Criteria is not null)
            {
                query = query.Where(specification.Criteria);
            }

            if(specification.OrderBy is not null)
            {
                query = query.OrderBy(specification.OrderBy);
            }
            else if(specification.OrderByDescending is not null)
            {
                query = query.OrderByDescending(specification.OrderByDescending);
            }

            if (specification.IsPagination)
            {
                query = query.Skip(specification.Skip).Take(specification.Take);
            }

            query = specification.Includes.Aggregate(query, (query, includeExpression) => query.Include(includeExpression));

            return query;
        }
    }
}
