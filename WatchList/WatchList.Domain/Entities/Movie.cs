using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WatchList.Domain.Events;

namespace WatchList.Domain.Entities
{
    public class Movie
    {
        public int TimesWatched { get; private set; }

        public void OnNext(WatchedMovieEvent evt)
        {
            TimesWatched = TimesWatched + 1;
        }
    }
}
