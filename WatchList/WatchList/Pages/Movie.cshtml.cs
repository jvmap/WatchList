using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WatchList.Data;

namespace WatchList.Pages
{
    public class MovieModel : PageModel
    {
        [FromQuery]
        public string Query { get; set; }

        public MovieData MovieData { get; set; }

        public UserMovieData UserMovieData { get; set; }

        private readonly IMovieRepository _movieRepository;

        public MovieModel(IMovieRepository movieRepository)
        {
            this._movieRepository = movieRepository;
        }

        public async Task OnGetAsync()
        {
            if (string.IsNullOrEmpty(Query)) return;

            MovieData = await _movieRepository.GetMovieByTitleAsync(Query);
        }
    }
}
