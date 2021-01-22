using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WatchList.Events;

namespace WatchList.Data
{
    public class SqlEventStore : IEventStore
    {
        private readonly DbContextOptions<SqlEventStoreDbContext> _options;

        public SqlEventStore(DbContextOptions<SqlEventStoreDbContext> options)
        {
            this._options = options;
            UpgradeDatabase();
        }

        private void UpgradeDatabase()
        {
            using (var db = new SqlEventStoreDbContext(_options))
            {
                db.Database.Migrate();
            }
        }

        public async Task AddEventAsync(IEvent evt)
        {
            using (var db = new SqlEventStoreDbContext(_options))
            {
                db.Events.Add(new Event
                {
                    AggregateId = evt.AggregateId,
                    Name = evt.Name,
                    EventData = SerializeEventData(evt.EventData),
                    TimeStamp = DateTimeOffset.Now
                });
                await db.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<IEvent>> GetEventsAsync()
        {
            using (var db = new SqlEventStoreDbContext(_options))
            {
                return await db.Events
                    .OrderBy(evt => evt.Id)
                    .Select(evt => new EventInfo(evt))
                    .ToListAsync();
            }
        }

        private string SerializeEventData(IEnumerable<(string, string)> eventData)
        {
            var dict = new Dictionary<string, string>(eventData
                .Select(kv => new KeyValuePair<string, string>(kv.Item1, kv.Item2)));

            string result = JsonConvert.SerializeObject(dict);
            return result;
        }

        class EventInfo : IEvent
        {
            public EventInfo(Event evt)
            {
                AggregateId = evt.AggregateId;
                Name = evt.Name;
                EventData = JsonConvert.DeserializeObject<Dictionary<string, string>>(evt.EventData)
                    .Select(kvp => (kvp.Key, kvp.Value))
                    .ToList();
                TimeStamp = evt.TimeStamp;
            }

            public string AggregateId { get; }

            public string Name { get; }

            public IEnumerable<(string, string)> EventData { get; }

            public DateTimeOffset TimeStamp { get; }
        }
    }
}
