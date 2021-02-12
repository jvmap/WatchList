using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace WatchList.Events
{
    public abstract class Event
    {
        string _name;
        IEnumerable<(string, string)> _eventData;

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

        public string AggregateId { get; init; }

        public DateTimeOffset Timestamp { get; init; }

        public IEnumerable<(string, string)> EventData
        {
            get
            {
                if (_eventData == null)
                {
                    _eventData = this.GetType()
                        .GetProperties()
                        .Where(p => p.DeclaringType != typeof(Event))
                        .Select(p => (ToCamelCase(p.Name), Convert.ToString(p.GetValue(this), CultureInfo.InvariantCulture)))
                        .ToList();
                }
                return _eventData;
            }
        }

        private static string ToCamelCase(string name)
        {
            if (string.IsNullOrEmpty(name))
                return name;
            else if (Char.IsLower(name[0]))
                return name;
            else
                return Char.ToLower(name[0]) + name.Substring(1);
        }
    }
}
