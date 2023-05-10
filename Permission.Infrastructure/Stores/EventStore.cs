using Confluent.Kafka;
using CQRS.Core.Domain;
using CQRS.Core.Events;
using CQRS.Core.Exceptions;
using CQRS.Core.Infrastructure;
using CQRS.Core.Producers;
using Permission.Domain.Aggregates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Permission.Infrastructure.Stores
{
    public class EventStore : IEventStore
    {
        private readonly IEventStoreRepository _eventStoreRepository;
        private readonly IEventProducer _eventProducer;

        public EventStore(IEventStoreRepository eventStoreRepository, IEventProducer eventProducer)
        {
            _eventStoreRepository = eventStoreRepository;
            _eventProducer = eventProducer;
        }

        public async Task<List<BaseEvent>> GetEventsAsync(int aggregateId)
        {
            var eventStream = await _eventStoreRepository.FindByAggregateId(aggregateId);

            if(eventStream == null || !eventStream.Any()) 
            {
                throw new AggregateNotFoundException("Incorrect permission Id");
            }

            return eventStream.OrderBy(x=> x.Version).Select(x => x.EventData).ToList();
        }

        public async Task SaveEventsAsync(int aggregateId, IEnumerable<BaseEvent> events, int expectedVersion)
        {
            var eventStream = await _eventStoreRepository.FindByAggregateId(aggregateId);

            if (expectedVersion != 1 && eventStream.Count > 0)
            {
                if(eventStream[^1]?.Version != expectedVersion)
                    throw new ConcurrencyException();

                throw new ConcurrencyException();
            }

            var version = expectedVersion;

            foreach(var @event in events)
            {
                version++;
                @event.Version = version;
                var eventType = @event.GetType().Name;
                var eventModel = new EventModel
                {
                    TimeStamp = DateTime.Now,
                    AggregateIdentifier = aggregateId,
                    AggregateType = nameof(PermissionAggregate),
                    Version = version,
                    EventType = eventType,
                    EventData = @event
                };

                await _eventStoreRepository.SaveAsync(eventModel);

                var topic = Environment.GetEnvironmentVariable("KAFKA_TOPIC");

                await _eventProducer.ProduceAsync(topic, @event);
            }
        }
    }
}
