using Microsoft.EntityFrameworkCore;
using Permission.Common.Domain.Entities;
using Permission.Common.Domain.Interfaces.Repositories;
using Permission.Common.Domain.Interfaces.Services;
using Permission.Common.Domain.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Permission.Common.Infrastructure.Repositories
{
    public class EntitySqlRepository<T> : IEntitySqlRepository<T>
        where T : BaseEntity
    {
        private readonly DbContext _dbContext;
        private readonly IEntityFrameworkBuilder<T> _entityFrameworkBuilder;

        protected EntitySqlRepository(DbContext context, IEntityFrameworkBuilder<T> entityFrameworkBuilder)
        {
            _dbContext = context;
            _entityFrameworkBuilder = entityFrameworkBuilder;
        }
        public virtual T Add(T entity)
        {
            return _dbContext.Set<T>().Add(entity).Entity;
        }

        public async Task<int> Count(List<Specification<T>>? specifications = null)
        {
            var baseQuery = _dbContext.Set<T>().AsQueryable();

            if (specifications == null)
            {
                return await baseQuery.CountAsync();
            }

            var whereExpression = _entityFrameworkBuilder.GetWhereExpression(specifications);
            if (whereExpression != null)
            {
                baseQuery = baseQuery.Where(whereExpression);
            }

            return await baseQuery.CountAsync();
        }

        protected async Task<IReadOnlyCollection<T>> GetAllWithInclude(IQueryable<T> baseQuery, Criteria<T> criteria)
        {
            var whereExpression = _entityFrameworkBuilder.GetWhereExpression(criteria.Specifications);
            if (whereExpression != null)
            {
                baseQuery = baseQuery.Where(whereExpression);
            }

            var result = await _entityFrameworkBuilder.ToListOrderedPagedValues(baseQuery, criteria);
            return result;
        }

        public virtual async Task<IReadOnlyCollection<T>> GetAll(Criteria<T>? criteria = null)
        {
            criteria ??= new Criteria<T>();

            var baseQuery = _dbContext.Set<T>().AsQueryable();

            var whereExpression = _entityFrameworkBuilder.GetWhereExpression(criteria.Specifications);
            if (whereExpression != null)
            {
                baseQuery = baseQuery.Where(whereExpression);
            }

            var result = await _entityFrameworkBuilder.ToListOrderedPagedValues(baseQuery, criteria);
            return result;
        }

        public virtual async Task<T?> GetById(int id)
        {
            return await _dbContext.Set<T>().FirstOrDefaultAsync(e => e.Id.Equals(id));
        }

        public virtual T Update(T entity)
        {
            return _dbContext.Set<T>().Update(entity).Entity;
        }
    }
}
