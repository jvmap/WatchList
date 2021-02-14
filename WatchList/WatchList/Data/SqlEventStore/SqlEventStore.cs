using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using WatchList.Domain.Events;

namespace WatchList.Data.SqlEventStore
{
    public class SqlEventStore : IEventStore
    {
        private readonly DbContextOptions<SqlEventStoreDbContext> _options;
        private readonly DtoConverter _dtoConverter;

        public SqlEventStore(DbContextOptions<SqlEventStoreDbContext> options)
        {
            this._options = options;
            UpgradeDatabase();
            this._dtoConverter = new DtoConverter();
        }

        private void UpgradeDatabase()
        {
            using (var db = new SqlEventStoreDbContext(_options))
            {
                db.Database.Migrate();
            }
        }

        public async Task AddEventsAsync(IEnumerable<Event> evts)
        {
            using (var db = new SqlEventStoreDbContext(_options))
            {
                foreach (Event evt in evts)
                {
                    EventDto dto = _dtoConverter.ToDto(evt);
                    dto.TimeStamp = DateTimeOffset.Now;
                    db.Events.Add(dto);
                }
                await db.SaveChangesAsync();
            }
        }

        public Task<IEnumerable<Event>> GetEventsAsync()
        {
            return PrivateGetEventsAsync();
        }

        public Task<IEnumerable<Event>> GetEventsAsync(string aggregateId)
        {
            return PrivateGetEventsAsync(where: evt => evt.AggregateId == aggregateId);
        }

        private async Task<IEnumerable<Event>> PrivateGetEventsAsync(Expression<Func<EventDto, bool>> where = null)
        {
            IEnumerable<EventDto> dtos;
            using (var db = new SqlEventStoreDbContext(_options))
            {
                IQueryable<EventDto> query = db.Events;
                if (where != null)
                    query = query.Where(where);
                query = query.OrderBy(evt => evt.Id);
                dtos = await query.ToListAsync();
            }
            return dtos
                .Select(evt => _dtoConverter.FromDto(evt))
                .ToList();
        }
    }
}
