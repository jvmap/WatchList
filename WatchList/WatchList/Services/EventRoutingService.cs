using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using WatchList.Data;
using WatchList.Events;

namespace WatchList.Services
{
    public class EventRoutingService : IHostedService
    {
        private readonly IEventBus _bus;
        private readonly IUserMovieRepository _repo;

        public EventRoutingService(IEventBus bus, IUserMovieRepository repo)
        {
            this._bus = bus;
            this._repo = repo;
        }
        
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _bus.SubscribeAsync(_repo);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
