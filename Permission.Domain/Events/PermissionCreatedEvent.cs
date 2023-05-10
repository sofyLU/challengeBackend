using CQRS.Core.Events;
using Permission.Common.Events.DTOs;
using Permission.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Permission.Common.Events
{
    public class PermissionCreatedEvent : BaseEvent
    {
        public PermissionCreatedEvent() : base(nameof(PermissionCreatedEvent))
        {
        }

        public string EmployeeName { get; set; }
        public string EmployeeSurName { get; set; }
        public int PermissionTypeId { get; set; }
        public DateTime created { get; set; }
    }
}
