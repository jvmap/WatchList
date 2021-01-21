using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WatchList.Data
{
    /// <summary>
    /// User-specific data about a particular movie.
    /// </summary>
    public class UserMovieData
    {
        public bool Watched { get; set; }

        /// <summary>
        /// Movie Rating 1-5 or null.
        /// </summary>
        public int? Rating { get; set; }

        public bool WantToWatch { get; set; }
    }
}
