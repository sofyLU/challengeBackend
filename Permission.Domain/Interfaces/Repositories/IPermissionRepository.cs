using Permission.Common.Domain.Interfaces.Repositories;
using Permission.Domain.Entities;
using Permission.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Permission.Domain.Interfaces.Repositories
{
    public interface IPermissionRepository : IRepository<PermissionEntity>, IEntitySqlRepository<PermissionEntity>
    {
    }
}
