using Permission.Common.Events;
using Permission.Common.Events.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Permission.Domain.Aggregates
{
    public class PermissionAggregate : AggregateRoot
    {
        public bool _active;
        private string _employeeName;
        private string _employeeSurName;
        private PermissionTypeDto _permissionType;
        private DateTime _created;

        public bool Active
        {
            get => _active; set => _active = value;
        }

        public PermissionAggregate()
        {

        }

        public PermissionAggregate(Guid id, string employeeName,  string employeeSurName, PermissionTypeDto permissionTypes)
        {
            RaiseEvent(new PermissionCreatedEvent
            {
                Id = id,
              EmployeeName = employeeName,
              EmployeeSurName = employeeSurName,
              PermissionType = permissionTypes,
              created = DateTime.Now,
            });
        }

        public void Apply(PermissionCreatedEvent @event)
        {
            _id = @event.Id;
            _active = true;
            _employeeName = @event.EmployeeName;
            _employeeName = @event.EmployeeSurName;
            _permissionType = @event.PermissionType;
            _created = @event.created;
        }

        public void ModifyPermission(string employeeName, string employeeSurName, PermissionTypeDto permissionTypes)
        {
            if (!_active)
            {
                throw new InvalidOperationException("Ypu cannot edit the employee of an inactive permission!");
            }

            if(string.IsNullOrEmpty(employeeName))
            {
                throw new InvalidOperationException($"The value of {nameof(employeeName)} cannot be null or empty. " +
                    $"Please provide a valid {nameof(employeeName)}");
            }

            if(string.IsNullOrEmpty(employeeSurName))
            {
                throw new InvalidOperationException($"The value of {nameof(employeeSurName)} cannot be null or empty. " +
                    $"Please provide a valid {nameof(employeeName)}");
            }

            if(permissionTypes.Id <= 0)
            {
                throw new InvalidOperationException($"The value of {nameof(permissionTypes.Id)} shoud be greater than 0. " +
                    $"Please provide a valid {nameof(permissionTypes.Id)}");
            }

            RaiseEvent(new PermissionUpdatedEvent
            {
                Id = _id,
                EmployeeName = employeeName,
                EmployeeSurName = employeeSurName,
                PermissionType = permissionTypes
            });
        }

        public void Apply(PermissionUpdatedEvent @event)
        {
            _id = @event.Id;
        }
    }
}
