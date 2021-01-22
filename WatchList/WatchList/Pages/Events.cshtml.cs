using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WatchList.Data;
using WatchList.Events;

namespace WatchList.Pages
{
    public class EventsModel : PageModel
    {
        private readonly IEventStore _eventStore;

        public IEnumerable<PageEventInfo> Events { get; set; }

        public EventsModel(IEventStore eventStore)
        {
            this._eventStore = eventStore;
        }
        
        public async Task OnGetAsync()
        {
            Events = (await _eventStore.GetEventsAsync())
                .Select(e => new PageEventInfo(e))
                .Reverse()
                .ToList();
        }

        public class PageEventInfo
        {
            public PageEventInfo(IEvent evt)
            {
                TimeStamp = evt.GetType().GetProperty("TimeStamp").GetValue(evt, null);
                AggregateId = evt.AggregateId;
                Name = evt.Name;
                EventData = evt.EventData;
            }

            public dynamic TimeStamp { get; }

            public string AggregateId { get; }

            public string Name { get; }

            public IEnumerable<(string, string)> EventData { get; }
        }
    }
}
