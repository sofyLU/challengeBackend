using CQRS.Core.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Permission.Common.Events
{
    public class PermissionTypeCreatedEvent : BaseEvent
    {
        public PermissionTypeCreatedEvent() : base(nameof(PermissionTypeCreatedEvent))
        {
        }

        public Guid Id { get; set; }
        public string Description { get; set; }
    }
}
