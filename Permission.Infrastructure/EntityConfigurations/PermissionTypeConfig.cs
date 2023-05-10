using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Permission.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Permission.Infrastructure.EntityConfigurations
{
    public class PermissionTypeConfig : IEntityTypeConfiguration<PermissionTypeEntity>
    {
        public void Configure(EntityTypeBuilder<PermissionTypeEntity> builder)
        {
            builder.ToTable("permissionTypes");

            builder.HasKey(x => x.Id);

            builder.Property(t => t.Id)
                .ValueGeneratedOnAdd();
        }
    }
}
