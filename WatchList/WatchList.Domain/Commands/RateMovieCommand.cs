using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WatchList.Domain.Commands;
using WatchList.Domain.Entities;

namespace WatchList.Domain.Commands
{
    public class RateMovieCommand : Command<Movie>
    {
        // Rating 1-5
        public int Rating { get; set; }
    }
}
