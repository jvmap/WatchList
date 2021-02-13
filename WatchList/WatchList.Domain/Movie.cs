using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WatchList.Domain
{
    public class Movie
    {
        public int TimesWatched { get; private set; }

        public void Watched()
        {
            TimesWatched = TimesWatched + 1;
        }
    }
}
