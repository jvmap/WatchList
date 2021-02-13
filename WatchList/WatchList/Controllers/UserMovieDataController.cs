using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WatchList.CommandHandlers;
using WatchList.Commands;
using WatchList.Data;
using WatchList.Events;
using WatchList.Messages;

namespace WatchList.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class UserMovieDataController : ControllerBase
    {
        private readonly IUserMovieRepository _repository;
        private readonly IEventStore _eventStore;
        private readonly IEventBus _eventBus;

        public UserMovieDataController(
            IUserMovieRepository repository,
            IEventStore eventStore,
            IEventBus eventBus
            )
        {
            this._repository = repository;
            this._eventStore = eventStore;
            this._eventBus = eventBus;
        }
        
        [HttpGet("{movieId}")]
        public Task<UserMovieData> GetAsync(string movieId)
        {
            return PrivateGetAsync(movieId);
        }

        [HttpPost("{movieId}/watched")]
        public async Task<UserMovieData> PostWatchedAsync(string movieId)
        {
            var handler = new WatchedMovieCommandHandler(_eventStore, _eventBus);
            await handler.HandleCommandAsync(new WatchedMovieCommand { MovieId = movieId });
            return await PrivateGetAsync(movieId);
        }

        [HttpPost("{movieId}/wantToWatch")]
        public async Task<UserMovieData> PostWantToWatchAsync(string movieId)
        {
            var handler = new WantToWatchMovieCommandHandler(_eventStore, _eventBus);
            await handler.HandleCommandAsync(new WantToWatchMovieCommand { MovieId = movieId });
            return await PrivateGetAsync(movieId);
        }

        [HttpPost("{movieId}/rate")]
        public async Task<UserMovieData> PostRateAsync(string movieId, [FromBody] RatingMessage msg)
        {
            var handler = new RateMovieCommandHandler(_eventStore, _eventBus);
            await handler.HandleCommandAsync(new RateMovieCommand { MovieId = movieId, Rating = msg.Rating });
            return await PrivateGetAsync(movieId);
        }

        private async Task<UserMovieData> PrivateGetAsync(string movieId)
        {
            UserMovieData result = await _repository.GetUserMovieDataByIdAsync(movieId)
                ?? new UserMovieData();
            return result;
        }
    }
}
