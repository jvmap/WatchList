using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
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

        public async Task AddEventAsync(Event evt)
        {
            using (var db = new SqlEventStoreDbContext(_options))
            {
                db.Events.Add(new EventDto
                {
                    AggregateId = evt.AggregateId,
                    Name = evt.Name,
                    EventData = SerializeEventData(evt.EventData),
                    TimeStamp = DateTimeOffset.Now
                });
                await db.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Event>> GetEventsAsync()
        {
            using (var db = new SqlEventStoreDbContext(_options))
            {
                return (await db.Events
                    .OrderBy(evt => evt.Id)
                    .ToListAsync())
                    .Select(ConstructEvent)
                    .ToList();
            }
        }

        public async Task<IEnumerable<Event>> GetEventsAsync(string aggregateId)
        {
            using (var db = new SqlEventStoreDbContext(_options))
            {
                return (await db.Events
                    .OrderBy(evt => evt.Id)
                    .Where(evt => evt.AggregateId == aggregateId)
                    .ToListAsync())
                    .Select(evt => ConstructEvent(evt))
                    .ToList();
            }
        }

        private Event ConstructEvent(EventDto evtDto)
        {
            string typeName = "WatchList.Events." + evtDto.Name + "Event";
            Type eventType = Type.GetType(typeName, throwOnError: true);
            Event evt = (Event)Activator.CreateInstance(eventType);
            eventType
                .GetProperty(nameof(evt.AggregateId))
                .SetValue(evt, evtDto.AggregateId);
            eventType
                .GetProperty(nameof(evt.Timestamp))
                .SetValue(evt, evtDto.TimeStamp);
            foreach ((string key, string value) in DeserializeEventData(evtDto.EventData))
            {
                PropertyInfo property = eventType.GetProperty(ToTitleCase(key));
                property.SetValue(evt, ConvertToType(value, property.PropertyType));
            }
            return evt;
        }

        private static readonly Dictionary<Type, Func<string, object>> _converters = new Dictionary<Type, Func<string, object>>();
        
        static SqlEventStore()
        {
            RegisterConverter(str => int.Parse(str, CultureInfo.InvariantCulture));
        }

        private static void RegisterConverter<T>(Func<string, T> converter)
        {
            _converters.Add(typeof(T), value => converter(value));
        }

        private static object ConvertToType(string value, Type type)
        {
            return _converters[type](value);
        }

        private static string ToTitleCase(string key)
        {
            if (string.IsNullOrEmpty(key))
                return key;
            else
                return Char.ToUpper(key[0]) + key.Substring(1);
        }

        private static string SerializeEventData(IEnumerable<(string, string)> eventData)
        {
            var dict = new Dictionary<string, string>(eventData
                .Select(kv => new KeyValuePair<string, string>(kv.Item1, kv.Item2)));

            string result = JsonConvert.SerializeObject(dict);
            return result;
        }

        private static IEnumerable<(string, string)> DeserializeEventData(string eventData)
        {
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(eventData)
                        .Select(kvp => (kvp.Key, kvp.Value))
                        .ToList();
        }
    }
}
