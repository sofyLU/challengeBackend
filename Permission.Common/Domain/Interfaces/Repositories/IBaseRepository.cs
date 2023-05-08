using Permission.Common.Domain.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Permission.Common.Domain.Interfaces.Repositories
{
    public interface IBaseRepository<T> where T : class
    {
        Task<IReadOnlyCollection<T>> GetAll(Criteria<T>? criteria = null);
        Task<int> Count(List<Specification<T>>? specifications = null);
    }
}
