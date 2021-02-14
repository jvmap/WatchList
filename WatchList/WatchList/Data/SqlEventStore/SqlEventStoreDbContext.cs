using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WatchList.Data.SqlEventStore
{
    public class SqlEventStoreDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public SqlEventStoreDbContext(DbContextOptions<SqlEventStoreDbContext> options)
            : base(options)
        {

        }
        
        public DbSet<EventDto> Events { get; set; }
    }
}
