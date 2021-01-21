using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WatchList.Data;

namespace WatchList.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class UserMovieDataController : ControllerBase
    {
        private readonly IUserMovieRepository _repository;

        public UserMovieDataController(IUserMovieRepository repository)
        {
            this._repository = repository;
        }
        
        [HttpGet("{movieId}")]
        public async Task<UserMovieData> GetAsync(string movieId)
        {
            return await _repository.GetUserMovieDataByIdAsync(movieId) 
                ?? new UserMovieData();
        }
    }
}
