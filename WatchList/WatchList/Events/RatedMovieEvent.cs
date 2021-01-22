using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace WatchList.Events
{
    public class RatedMovieEvent : Event
    {
        private readonly string _movieId;
        private readonly int _rating;

        public RatedMovieEvent(string movieId, int rating)
        {
            this._movieId = movieId;
            this._rating = rating;
        }

        public override string AggregateId => _movieId;

        public override IEnumerable<(string, string)> EventData
        {
            get
            {
                yield return ("rating", _rating.ToString(CultureInfo.InvariantCulture));
            }
        }
    }
}
