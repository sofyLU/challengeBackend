using CQRS.Core.Domain;
using CQRS.Core.Events;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Permission.Infrastructure.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Permission.Infrastructure.Repositories
{
    public class EventStoryRepository : IEventStoreRepository
    {
        private readonly IMongoCollection<EventModel> _eventStoreCollection;

        public EventStoryRepository(IOptions<MongoDbConfig> config)
        {
            var mongoClient = new MongoClient(config.Value.ConnectionString);
            var mongoDataBase = mongoClient.GetDatabase(config.Value.Database);

            _eventStoreCollection = mongoDataBase.GetCollection<EventModel>(config.Value.Collection);
        }

        public async Task<List<EventModel>> FindByAggregateId(int aggregateId)
        {
            var result = await _eventStoreCollection.Find(x => x.AggregateIdentifier == aggregateId).ToListAsync().ConfigureAwait(false);

            return result;
        }

        public async Task SaveAsync(EventModel @event)
        {
            await _eventStoreCollection.InsertOneAsync(@event).ConfigureAwait(false); 
        }
    }
}
