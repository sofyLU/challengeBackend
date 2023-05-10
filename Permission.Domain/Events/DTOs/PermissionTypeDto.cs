using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Permission.Common.Events.DTOs
{
    public class PermissionTypeDto
    {
        public int Id { get; set; }
        public string Description { get; set; }

        public PermissionTypeDto(int id, string description)
        {
            Id = id;
            Description = description;
        }
    }
}
