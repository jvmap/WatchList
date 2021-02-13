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
        private readonly CommandInvoker _invoker;

        public UserMovieDataController(
            IUserMovieRepository repository,
            CommandInvoker invoker
            )
        {
            this._repository = repository;
            this._invoker = invoker;
        }
        
        [HttpGet("{movieId}")]
        public Task<UserMovieData> GetAsync(string movieId)
        {
            return PrivateGetAsync(movieId);
        }

        [HttpPost("{movieId}/watched")]
        public async Task<UserMovieData> PostWatchedAsync(string movieId)
        {
            var handler = new WatchedMovieCommandHandler();
            var cmd = new WatchedMovieCommand { AggregateId = movieId };
            await _invoker.InvokeAsync(cmd, handler);
            return await PrivateGetAsync(movieId);
        }

        [HttpPost("{movieId}/wantToWatch")]
        public async Task<UserMovieData> PostWantToWatchAsync(string movieId)
        {
            var handler = new WantToWatchMovieCommandHandler();
            var cmd = new WantToWatchMovieCommand { AggregateId = movieId };
            await _invoker.InvokeAsync(cmd, handler);
            return await PrivateGetAsync(movieId);
        }

        [HttpPost("{movieId}/rate")]
        public async Task<UserMovieData> PostRateAsync(string movieId, [FromBody] RatingMessage msg)
        {
            var handler = new RateMovieCommandHandler();
            var cmd = new RateMovieCommand { AggregateId = movieId, Rating = msg.Rating };
            await _invoker.InvokeAsync(cmd, handler);
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
