using CQRS.Core.Handlers;
using Permission.Domain.Aggregates;
using Permission.Domain.Entities;
using Permission.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Permission.Application.Commands
{
    public class CommandHandler : ICommandHandler
    {
        private readonly IEventSourcingHandler<PermissionAggregate> _eventSourcingHandler;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IPermissionTypeRepository _permissionTypeRepository;

        public CommandHandler(IEventSourcingHandler<PermissionAggregate> eventSourcingHandler,
            IPermissionRepository permissionRepository,
            IPermissionTypeRepository permissionTypeRepository)
        {
            _eventSourcingHandler = eventSourcingHandler;
            _permissionRepository = permissionRepository;
            _permissionTypeRepository = permissionTypeRepository;
        }

        public async Task<int> HandleAsync(RequestPermissionCommand command)
        {
            var permissionType = await _permissionTypeRepository.GetById(command.PermissionTypeId);

            var permission = new PermissionEntity
            {
                EmployeeForename = command.EmployeeName,
                EmployeeSurname = command.EmployeeSurName,
                PermissionDate = DateTime.Now,
                PermissionType = permissionType
            };

            var dbPermission = _permissionRepository.Add(permission);

            await _permissionRepository.UnitOfWork.SaveEntitiesAsync();
            command.Id = dbPermission.Id;

            var aggregate = new PermissionAggregate(dbPermission.Id, command.EmployeeName, command.EmployeeSurName, command.PermissionTypeId);

            await _eventSourcingHandler.SaveAsync(aggregate);

            return aggregate.Id;
        }

        public async Task<int> HandleAsync(ModifyPermissionCommand command)
        {
            var aggregate = await _eventSourcingHandler.GetByIdAsync(command.Id);

            var permission = await _permissionRepository.GetById(command.Id);

            var permissionType = await _permissionTypeRepository.GetById(command.PermissionTypeId);

            permission.EmployeeForename = command.EmployeeName;
            permission.EmployeeSurname = command.EmployeeSurName;
            permission.PermissionType = permissionType;
            permission.PermissionDate = DateTime.Now;

            _permissionRepository.Update(permission);

            await _permissionRepository.UnitOfWork.SaveChangesAsync();

            aggregate.ModifyPermission(command.EmployeeName, command.EmployeeSurName, command.PermissionTypeId);

            await _eventSourcingHandler.SaveAsync(aggregate);

            return aggregate.Id;
        }
    }
}
