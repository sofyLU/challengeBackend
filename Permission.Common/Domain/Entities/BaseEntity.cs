using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Permission.Common.Domain.Entities
{
    public class BaseEntity
    {
        [Comment("Unique Id")]
        [Column(TypeName = "integer")]
        [Key]
        public int Id { get; set; }

        protected BaseEntity(int id) 
        {
            Id = id;
        }
    }
}
