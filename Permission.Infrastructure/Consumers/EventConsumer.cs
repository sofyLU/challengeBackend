using Confluent.Kafka;
using CQRS.Core.Consumers;
using CQRS.Core.Events;
using Microsoft.Extensions.Options;
using Permission.Domain.Interfaces;
using Permission.Infrastructure.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Permission.Infrastructure.Consumers
{
    public class EventConsumer : IEventConsumer
    {
        private readonly ConsumerConfig _config;
        private readonly IEventHandler _eventHandler;

        public EventConsumer(IOptions<ConsumerConfig> config, IEventHandler eventHandler)
        {
            _config = config.Value;
            _eventHandler = eventHandler;
        }

        public void Consume(string topic)
        {
            using var consumer = new ConsumerBuilder<string, string>(_config)
                .SetKeyDeserializer(Deserializers.Utf8)
                .SetValueDeserializer(Deserializers.Utf8)
                .Build();

            while(true)
            {
                var consumerResult = consumer.Consume();

                if (consumerResult?.Message == null) continue;

                var options = new JsonSerializerOptions { Converters = { new EventJsonConvert() } };

                var @event = JsonSerializer.Deserialize<BaseEvent>(consumerResult.Message.Value, options);

                var handlerMethod = _eventHandler.GetType().GetMethod("On", new Type[] { @event.GetType() });

                if(handlerMethod == null)
                {
                    throw new ArgumentNullException(nameof(handlerMethod), "Could not find event handler method!");
                }

                handlerMethod.Invoke(_eventHandler, new object[] { @event });

                consumer.Commit(consumerResult);
            }
        }
    }
}
