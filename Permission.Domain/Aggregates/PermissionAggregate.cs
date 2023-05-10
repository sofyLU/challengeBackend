using Permission.Common.Events;
using Permission.Domain.Entities;
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
        private int _permissionTypeId;
        private DateTime _created;

        public bool Active
        {
            get => _active; set => _active = value;
        }

        public PermissionAggregate()
        {

        }

        public PermissionAggregate(int id, string employeeName,  string employeeSurName, int permissionTypeId)
        {
            RaiseEvent(new PermissionCreatedEvent
            {
                Id = id,
              EmployeeName = employeeName,
              EmployeeSurName = employeeSurName,
              PermissionTypeId = permissionTypeId,
              created = DateTime.Now,
            });
        }

        public void Apply(PermissionCreatedEvent @event)
        {
            _id = @event.Id;
            _active = true;
            _employeeName = @event.EmployeeName;
            _employeeSurName = @event.EmployeeSurName;
            _permissionTypeId = @event.PermissionTypeId;
            _created = @event.created;
        }

        public void ModifyPermission(string employeeName, string employeeSurName, int permissionTypeId)
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

            if(permissionTypeId <= 0)
            {
                throw new InvalidOperationException($"The value of {nameof(permissionTypeId)} shoud be greater than 0. " +
                    $"Please provide a valid {nameof(permissionTypeId)}");
            }

            RaiseEvent(new PermissionUpdatedEvent
            {
                Id = _id,
                EmployeeName = employeeName,
                EmployeeSurName = employeeSurName,
                PermissionTypeId = permissionTypeId
            });
        }

        public void Apply(PermissionUpdatedEvent @event)
        {
            _id = @event.Id;
        }
    }
}
