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
using WatchList.Services;

namespace WatchList.Data.SqlEventStore
{
    public class SqlEventPersistence : IEventPersistence
    {
        private readonly DbContextOptions<SqlEventPersistenceDbContext> _options;
        private readonly DtoConverter _dtoConverter;

        public long MinIndex => 1;
        public int DefaultBatchSize => 1024;

        public SqlEventPersistence(DbContextOptions<SqlEventPersistenceDbContext> options)
        {
            this._options = options;
            this._dtoConverter = new DtoConverter();
            UpgradeDatabase();
        }

        private void UpgradeDatabase()
        {
            using (var db = new SqlEventPersistenceDbContext(_options))
            {
                db.Database.Migrate();
            }
        }

        public async Task<IReadOnlyCollection<EventEnvelope>> AddEventsAsync(IReadOnlyCollection<EventEnvelope> envelopes)
        {
            List<EventDto> dtos = envelopes
                .Select(_dtoConverter.ToDto)
                .ToList();
            using (var db = new SqlEventPersistenceDbContext(_options))
            {
                db.Events.AddRange(dtos);
                await db.SaveChangesAsync();
            }
            return dtos
                .Select(_dtoConverter.FromDto)
                .ToList();
        }

        public Task GetEventsAsync(long fromIndex, AsyncEventProcessor processBatchAsync, int batchSize)
        {
            if (fromIndex < MinIndex) throw new ArgumentOutOfRangeException(nameof(fromIndex));
            if (batchSize <= 0) throw new ArgumentOutOfRangeException(nameof(batchSize));
            return PrivateGetEventsAsync(fromIndex, processBatchAsync, batchSize);
        }

        public Task GetEventsAsync(long fromIndex, string aggregateId, AsyncEventProcessor processBatchAsync, int batchSize)
        {
            if (fromIndex < MinIndex) throw new ArgumentOutOfRangeException(nameof(fromIndex));
            if (batchSize <= 0) throw new ArgumentOutOfRangeException(nameof(batchSize));
            return PrivateGetEventsAsync(fromIndex, processBatchAsync, batchSize, where: evt => evt.AggregateId == aggregateId);
        }

        private async Task PrivateGetEventsAsync(
            long fromIndex,
            AsyncEventProcessor processBatchAsync,
            int batchSize,
            Expression<Func<EventDto, bool>> where = null)
        {
            while (true)
            {
                List<EventEnvelope> batch = await GetBatchAsync(fromIndex, batchSize, where);
                if (batch.Count == 0)
                    break;
                await processBatchAsync(batch);
                fromIndex += batch.Count;
            }
        }

        private async Task<List<EventEnvelope>> GetBatchAsync(
            long fromIndex,
            int batchSize,
            Expression<Func<EventDto, bool>> where = null)
        {
            List<EventEnvelope> results = new List<EventEnvelope>(batchSize);
            using (var db = new SqlEventPersistenceDbContext(_options))
            {
                IQueryable<EventDto> query = db.Events;
                if (fromIndex > MinIndex)
                    query = query.Where(evt => evt.Id >= fromIndex);
                if (where != null)
                    query = query.Where(where);
                query = query
                    .OrderBy(evt => evt.Id)
                    .Take(batchSize);
                await foreach (EventDto dto in query.AsAsyncEnumerable())
                    results.Add(_dtoConverter.FromDto(dto));
            }
            return results;
        }
    }
}
