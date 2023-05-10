using Microsoft.EntityFrameworkCore;
using Permission.Common.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Permission.Domain.Entities
{
    public class PermissionTypeEntity : BaseEntity
    {
        [Column(TypeName = "text")]
        [Comment("Permission Description")]
        public string Description { get; set; }
        [JsonIgnore]
        public virtual List<PermissionEntity> Permissions { get; set; }

        public PermissionTypeEntity() 
        { 
        }

        protected PermissionTypeEntity(int id) : base(id) 
        {
        }

    }
}
