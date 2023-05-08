using CQRS.Core.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Permission.Application.Commands
{
    public class RequestPermissionCommand : BaseCommand
    {
        public string EmployeeName { get; set; }
        public string EmployeeSurName { get; set; }
        public int PermissionType { get; set; }
    }
}
