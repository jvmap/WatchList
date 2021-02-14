using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WatchList.Services
{
    public interface IClock    
    {
        public DateTimeOffset Now();
    }
}
