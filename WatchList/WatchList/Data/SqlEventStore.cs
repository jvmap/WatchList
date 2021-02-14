using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
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
                        EventData = SerializeEventData(evt),
                        TimeStamp = DateTimeOffset.Now
                    });
                }
                await db.SaveChangesAsync();
            }
        }

        public Task<IEnumerable<Event>> GetEventsAsync()
        {
            return PrivateGetEventsAsync();
        }

        public Task<IEnumerable<Event>> GetEventsAsync(string aggregateId)
        {
            return PrivateGetEventsAsync(where: evt => evt.AggregateId == aggregateId);
        }

        private async Task<IEnumerable<Event>> PrivateGetEventsAsync(Expression<Func<EventDto, bool>> where = null)
        {
            IEnumerable<EventDto> dtos;
            using (var db = new SqlEventStoreDbContext(_options))
            {
                IQueryable<EventDto> query = db.Events;
                if (where != null)
                    query = query.Where(where);
                query = query.OrderBy(evt => evt.Id);
                dtos = await query.ToListAsync();
            }
            return dtos
                .Select(evt => ConstructEvent(evt))
                .ToList();
        }

        private static string GetEventName(Event evt)
        {
            string name = evt.GetType().Name;
            if (!name.EndsWith("Event"))
                throw new ArgumentException("Event classnames must end with \"Event\". Got: " + evt.GetType().Name);
            name = name[0..^5];
            return name;
        }

        private static string SerializeEventData(Event evt)
        {
            var dict = evt
                .GetType()
                .GetProperties()
                .Where(p => p.DeclaringType != typeof(Event))
                .ToDictionary(p => ToCamelCase(p.Name), p => p.GetValue(evt));
            return JsonConvert.SerializeObject(dict);
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
            PopulateEventData(evtDto.EventData, evt);
            return evt;
        }

        private static void PopulateEventData(string eventData, Event evt)
        {
            JsonConvert.PopulateObject(eventData, evt);
        }
    }
}
