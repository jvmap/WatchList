﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WatchList.Events
{
    public struct ConcurrencyToken
    {
        internal int Version { get; init; }
    }
}
