using Permission.Common.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Permission.Domain.Aggregates
{
    internal class PermissionTypeAggregate : AggregateRoot
    {
        public bool _active;
        private string _description;

        public bool Active
        {
            get => _active; set => _active = value;
        }

        public PermissionTypeAggregate()
        {

        }

        public PermissionTypeAggregate(Guid id, string description)
        {
            RaiseEvent(new PermissionTypeCreatedEvent
            {
                Id = id,
                Description = description
            });
        }

        public void Apply(PermissionTypeCreatedEvent @event)
        {
            _id = @event.Id;
            _active = true;
            _description = @event.Description;
        }

        public void EditPermissionType(string description)
        {
            if (!_active)
            {
                throw new InvalidOperationException("Ypu cannot edit the description of an inactive permission type!");
            }

            if (string.IsNullOrEmpty(description))
            {
                throw new InvalidOperationException($"The value of {nameof(description)} cannot be null or empty. " +
                    $"Please provide a valid {nameof(description)}");
            }

            RaiseEvent(new PermissionTypeUpdatedEvent 
            { 
                Id = _id, 
                Description = description 
            });
        }

        public void Apply(PermissionTypeUpdatedEvent @event)
        {
            _id = @event.Id;
        }

        public void DeletePermissionType()
        {
            if (!_active)
            {
                throw new InvalidOperationException("The permission type has already removed!");
            }

            RaiseEvent(new PermissionTypeDeletedEvent
            {
                Id = _id
            });
        }

        public void Apply(PermissionTypeDeletedEvent @event)
        {
            _id = @event.Id;
            _active = false;
        }
    }
}
