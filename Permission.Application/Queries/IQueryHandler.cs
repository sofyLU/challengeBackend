using Permission.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Permission.Application.Queries
{
    public interface IQueryHandler
    {
        Task<List<PermissionEntity>> HandleAsync(GetAllPermissionsQuery query);
    }
}
