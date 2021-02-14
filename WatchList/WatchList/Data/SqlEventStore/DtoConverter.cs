using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WatchList.Domain.Events;

namespace WatchList.Data.SqlEventStore
{
    public class DtoConverter
    {
        public EventDto ToDto(Event evt)
        {
            return new EventDto
            {
                AggregateId = evt.AggregateId,
                Name = GetEventName(evt),
                EventData = SerializeEventData(evt),
                TimeStamp = evt.Timestamp
            };
        }

        public Event FromDto(EventDto evtDto)
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

        private static void PopulateEventData(string eventData, Event evt)
        {
            JsonConvert.PopulateObject(eventData, evt);
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
    }
}
