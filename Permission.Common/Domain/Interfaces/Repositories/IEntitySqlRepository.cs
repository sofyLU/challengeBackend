using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Permission.Common.Domain.Interfaces.Repositories
{
    public interface IEntitySqlRepository<T> : IBaseRepository<T>
        where T : class
    {
        Task<T?> GetById(Guid id);
        T Add(T entity);
        T Update(T entity);
    }
}
