using Microsoft.EntityFrameworkCore;
using Permission.Domain.Entities;
using Permission.Domain.SeedWork;
using Permission.Infrastructure.EntityConfigurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Permission.Infrastructure.DataAccess
{
    public class DataBaseContext : DbContext, IUnitOfWork
    { 
        public DataBaseContext(DbContextOptions options) : base(options) 
        { 
        }

        public DbSet<PermissionEntity> Permissions { get; set; }
        public DbSet<PermissionTypeEntity> PermissionTypes { get; set; }

        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
        {
            var result = await base.SaveChangesAsync(cancellationToken);
            return true;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new PermissionConfig());
        }
    }
}
