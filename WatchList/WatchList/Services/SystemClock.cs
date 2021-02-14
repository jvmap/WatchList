using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WatchList.Services
{
    public class SystemClock : IClock
    {
        public DateTimeOffset Now()
        {
            return DateTimeOffset.Now;
        }
    }
}
