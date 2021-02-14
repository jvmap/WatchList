using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using WatchList.Domain.Events;

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

        public async Task AddEventsAsync(IEnumerable<Event> evts)
        {
            using (var db = new SqlEventStoreDbContext(_options))
            {
                foreach (Event evt in evts)
                {
                    db.Events.Add(new EventDto
                    {
                        AggregateId = evt.AggregateId,
                        Name = GetEventName(evt),
                        EventData = SerializeEventData(GetEventData(evt)),
                        TimeStamp = DateTimeOffset.Now
                    });
                }
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

        private static string GetEventName(Event evt)
        {
            string name = evt.GetType().Name;
            if (!name.EndsWith("Event"))
                throw new ArgumentException("Event classnames must end with \"Event\". Got: " + evt.GetType().Name);
            name = name[0..^5];
            return name;
        }

        private static IEnumerable<(string, string)> GetEventData(Event evt)
        {
            return evt
                .GetType()
                .GetProperties()
                .Where(p => p.DeclaringType != typeof(Event))
                .Select(p => (ToCamelCase(p.Name), Convert.ToString(p.GetValue(evt), CultureInfo.InvariantCulture)))
                .ToList();
        }

        private static string ToCamelCase(string name)
        {
            if (string.IsNullOrEmpty(name))
                return name;
            else if (Char.IsLower(name[0]))
                return name;
            else
                return Char.ToLower(name[0]) + name.Substring(1);
        }

        private Event ConstructEvent(EventDto evtDto)
        {
            string typeName = "WatchList.Domain.Events." + evtDto.Name + "Event,WatchList.Domain";
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
