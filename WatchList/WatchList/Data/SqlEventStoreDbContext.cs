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
        
        public DbSet<EventDto> Events { get; set; }
    }

    public class EventDto
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string AggregateId { get; set; }

        public string Name { get; set; }

        public string EventData { get; set; }

        public DateTimeOffset TimeStamp { get; set; }
    }
}
