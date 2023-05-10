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
    public class PermissionEntity : BaseEntity
    {
        [Column(TypeName = "text")]
        [Comment("Employee Forename")]
        public string EmployeeForename { get; set; }

        [Column(TypeName = "text")]
        [Comment("Employe Surname")]
        public string EmployeeSurname { get; set; }

        [Column(TypeName = "date")]
        [Comment("Permission granted on Date")]
        public DateTime PermissionDate { get; set; }

        [Comment("Permission Type")]
        [JsonIgnore]
        public virtual PermissionTypeEntity PermissionType { get; set; }

        public PermissionEntity()
        {
        }
        protected PermissionEntity(int id) : base(id) 
        {
        }

       
    }
}
