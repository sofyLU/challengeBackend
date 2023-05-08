using Permission.Common.Domain.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Permission.Common.Domain.Interfaces.Services
{
    public interface IEntityFrameworkBuilder<T> where T : class
    {
        Expression<Func<T, bool>>? GetWhereExpression(List<Specification<T>> specifications);

        Task<List<T>> ToListOrderedPagedValues(IQueryable<T> query, Criteria<T> criteria);
    }
}
