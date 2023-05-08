using Microsoft.EntityFrameworkCore;
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
    public class PermissionRepository : EntitySqlRepository<PermissionEntity>, IPermissionRepository
    {
        private readonly DataBaseContext _context;

        public PermissionRepository(DataBaseContext context, 
            IEntityFrameworkBuilder<PermissionEntity> entityFrameworkBuilder)
            : base(context, entityFrameworkBuilder)
        {
            _context = context;
        }

        public IUnitOfWork UnitOfWork => _context;
    }
}
