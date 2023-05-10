using Permission.Common.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Permission.Domain.Interfaces
{
    public interface IEventHandler
    {
        Task On(CancellationToken cancellationToken);
        Task On(PermissionUpdatedEvent @event, CancellationToken cancellation);
        // Task On(PermissionGotEvent @event);
    }
}
