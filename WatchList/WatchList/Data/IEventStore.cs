﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WatchList.Events;

namespace WatchList.Data
{
    public interface IEventStore
    {
        void AddEvent(IEvent evt);
    }
}
