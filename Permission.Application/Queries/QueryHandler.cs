using Permission.Common.Domain.Specification;
using Permission.Domain.Entities;
using Permission.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Permission.Application.Queries
{
    public class QueryHandler : IQueryHandler
    {
        private readonly IPermissionRepository _permissionRepository;

        public QueryHandler(IPermissionRepository permissionRepository)
        {
            _permissionRepository = permissionRepository;
        }

        public async Task<List<PermissionEntity>> HandleAsync(GetAllPermissionsQuery query)
        {
            var criteria = new Criteria<PermissionEntity>(query.Filters, query.OrderBy, query.Page, query.PageSize);

            var permissions = await _permissionRepository.GetAll(criteria);

            return permissions.ToList();
        }
    }
}
