using Permission.Common.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Permission.Common.Domain.Interfaces.Repositories
{
    public interface IEntitySqlRepository<T> : IBaseRepository<T>
        where T : BaseEntity
    {
        Task<T?> GetById(int id);
        T Add(T entity);
        T Update(T entity);
    }
}
