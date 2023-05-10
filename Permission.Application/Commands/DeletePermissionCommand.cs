using CQRS.Core.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Permission.Application.Commands
{
    public class DeletePermissionCommand : BaseCommand
    {
        public int PermissionId { get; set; }
        public string EmployeeName { get; set; }    
    }
}
