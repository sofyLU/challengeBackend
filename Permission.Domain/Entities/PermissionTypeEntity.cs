using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Permission.Domain.Entities
{
    public class PermissionTypeEntity
    {
        [Comment("Unique Id")]
        [Column(TypeName = "integer")]
        [Key]
        public int Id { get; set; }

        [Column(TypeName = "text")]
        [Comment("Permission Description")]
        public string Description { get; set; }
        public virtual List<PermissionEntity> Permissions { get; set; }

    }
}
