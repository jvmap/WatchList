using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WatchList.Data;
using WatchList.Domain.Events;

namespace WatchList.Pages
{
    public class EventsModel : PageModel
    {
        private readonly IEventPersistence _persistence;

        public IEnumerable<EventEnvelope> Events { get; set; }

        public EventsModel(IEventPersistence persistence)
        {
            this._persistence = persistence;
        }
        
        public async Task OnGetAsync()
        {
            var events = new Stack<EventEnvelope>();
            await _persistence.GetEventsAsync(newEvents =>
            {
                foreach (EventEnvelope newEvent in newEvents)
                    events.Push(newEvent);
                return Task.CompletedTask;
            });
            Events = events;
        }

        public string GetEventName(EventEnvelope envelope)
        {
            return GetEventName(envelope.Event);
        }

        private string GetEventName(Event evt)
        {
            string name = evt.GetType().Name;
            if (name.EndsWith("Event"))
                name = name[0..^5];
            return name;
        }

        public IEnumerable<(string, string)> GetEventData(EventEnvelope envelope)
        {
            return GetEventData(envelope.Event);
        }

        private IEnumerable<(string, string)> GetEventData(Event evt)
        {
            return evt
                .GetType()
                .GetProperties()
                .Where(p => p.DeclaringType != typeof(Event))
                .Select(p => (p.Name, Convert.ToString(p.GetValue(evt))))
                .ToList();
        }
    }
}
