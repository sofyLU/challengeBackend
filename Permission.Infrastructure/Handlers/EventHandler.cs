using Permission.Common.Events;
using Permission.Domain.Interfaces;
using Permission.Domain.Entities;
using Permission.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Permission.Infrastructure.Handlers
{
    public class EventHandler : IEventHandler
    {
        private readonly IPermissionRepository _permissionRepository;
        private readonly IPermissionTypeRepository _permissionTypeRepository;

        public EventHandler(IPermissionRepository permissionRepository, IPermissionTypeRepository permissionTypeRepository)
        {
            _permissionRepository = permissionRepository;
            _permissionTypeRepository = permissionTypeRepository;
        }

        public async Task On(CancellationToken cancellationToken)
        {
            await _permissionRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }

        public async Task On(PermissionUpdatedEvent @event, CancellationToken cancellationToken)
        {
            var permission = await _permissionRepository.GetById(@event.Id);

            if (permission == null) { return; }

            await _permissionRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
