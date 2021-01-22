using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WatchList.Data
{
    public class SqlEventStoreDbContext : DbContext
    {
        public SqlEventStoreDbContext(DbContextOptions<SqlEventStoreDbContext> options)
            : base(options)
        {

        }
        
        public DbSet<Event> Events { get; set; }
    }

    public class Event
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string AggregateId { get; set; }

        public string Name { get; set; }

        public string EventData { get; set; }

        public DateTimeOffset TimeStamp { get; set; }
    }
}
