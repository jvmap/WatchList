using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WatchList.Data;

namespace WatchList.ViewComponents
{
    public class UserMovieDataViewComponent : ViewComponent
    {
        private readonly IUserMovieRepository _repository;

        public UserMovieDataViewComponent(IUserMovieRepository repository)
        {
            this._repository = repository;
        }
        
        public async Task<IViewComponentResult> InvokeAsync(string movieId)
        {
            var userMovieData = await _repository.GetUserMovieDataByIdAsync(movieId) ?? new UserMovieData();
            return View(userMovieData);
        }
    }
}
