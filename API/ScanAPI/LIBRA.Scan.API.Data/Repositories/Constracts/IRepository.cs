﻿using System.Linq.Expressions;

namespace LIBRA.Scan.API.Data.Repositories.Constracts
{
    public interface IRepository<TEntity> where TEntity : class
    {
        Task<IReadOnlyList<TEntity>> ListAsync(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null);

        Task<TEntity> AddAsync(TEntity entity, bool ensureTransaction);

        Task<TEntity> UpdateAsync(TEntity entity);

        Task<TEntity> DeleteAsync(TEntity entity);

        IQueryable<TEntity> GetWithInclude(Expression<Func<TEntity, bool>> predicate, params string[] include);

        Task<bool> AddRangeAsync(IEnumerable<TEntity> entity);

        Task<TEntity> RemoveAsync(TEntity entity, bool ensureTransaction);
    }
}