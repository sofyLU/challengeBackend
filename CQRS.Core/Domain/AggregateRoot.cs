﻿using CQRS.Core.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Permission.Domain
{
    public abstract class AggregateRoot
    {
        protected int _id;
        private readonly List<BaseEvent> _changes = new();
        public int Id
        {
            get { return _id; }
        }

        public int Version { get; set; } = -1;

        public IEnumerable<BaseEvent> GetUncommittedChanges()
        {
            return _changes;
        }

        public void MarkChangesAsCommitted()
        {
            _changes.Clear();
        }

        public void ApplyChange(BaseEvent @event, bool isNew)
        {
            var method = this.GetType().GetMethod("Apply", new Type[] { @event.GetType() });

            if(method == null)
            {
                throw new ArgumentNullException(nameof(method), $"The Apply method was not found int aggegate for {@event.GetType().Name}!");
            }

            method.Invoke(this, new object[] { @event });

            if(isNew )
            {
                _changes.Add( @event );
            }
        }

        protected void RaiseEvent(BaseEvent @event)
        {
            ApplyChange(@event, true);
        }

        public void ReplayEvents(IEnumerable<BaseEvent> events)
        {
            foreach(var @event in events) 
            { 
                ApplyChange( @event, false);
            }
        }
    }
}
