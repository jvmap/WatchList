﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WatchList.Events
{
    public abstract class Event: IEvent
    {
        string _name;
        
        public string Name
        {
            get
            {
                if (_name == null)
                {
                    string name = this.GetType().Name;
                    if (name.EndsWith("Event"))
                        name = name[0..^5];
                    _name = name;
                }
                return _name;
            }
        }

        public abstract string AggregateId { get; }
        public abstract IEnumerable<(string, string)> EventData { get; }
    }
}