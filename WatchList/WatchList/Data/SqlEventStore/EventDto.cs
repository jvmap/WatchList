using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace WatchList.Data.SqlEventStore
{
    public class EventDto
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string AggregateId { get; set; }

        public int Version { get; set; }

        public string Name { get; set; }

        public string EventData { get; set; }

        public DateTimeOffset TimeStamp { get; set; }
    }
}
