using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WatchList.Commands
{
    public class RateMovieCommand : Command
    {
        // Rating 1-5
        public int Rating { get; set; }
    }
}
