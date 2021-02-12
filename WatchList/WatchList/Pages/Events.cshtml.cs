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

        public IEnumerable<Event> Events { get; set; }

        public EventsModel(IEventStore eventStore)
        {
            this._eventStore = eventStore;
        }
        
        public async Task OnGetAsync()
        {
            Events = (await _eventStore.GetEventsAsync())
                .Reverse()
                .ToList();
        }
    }
}
