using Permission.Common.Domain.Interfaces.Services;
using Permission.Common.Infrastructure.Repositories;
using Permission.Domain.Entities;
using Permission.Domain.Interfaces.Repositories;
using Permission.Domain.SeedWork;
using Permission.Infrastructure.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Permission.Infrastructure.Repositories
{
    internal class PermissionTypeRepository : EntitySqlRepository<PermissionTypeEntity>, IPermissionTypeRepository
    {
        private readonly DataBaseContext _context;

        public PermissionTypeRepository(DataBaseContext context,
            IEntityFrameworkBuilder<PermissionTypeEntity> entityFrameworkBuilder)
            : base(context, entityFrameworkBuilder)
        {
            _context = context;
        }

        public IUnitOfWork UnitOfWork => _context;
    }
}
