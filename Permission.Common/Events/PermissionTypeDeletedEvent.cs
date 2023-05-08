using CQRS.Core.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Permission.Common.Events
{
    public class PermissionTypeDeletedEvent : BaseEvent
    {
        public PermissionTypeDeletedEvent() : base(nameof(PermissionTypeDeletedEvent))
        {

        }

        public Guid Id { get; set; }
    }
}
