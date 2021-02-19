using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WatchList.Data.SqlEventStore
{
    public class SqlEventPersistenceDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public SqlEventPersistenceDbContext(DbContextOptions<SqlEventPersistenceDbContext> options)
            : base(options)
        {

        }
        
        public DbSet<EventDto> Events { get; set; }
    }
}
