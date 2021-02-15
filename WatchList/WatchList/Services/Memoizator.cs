using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WatchList.Services
{
    public class Memoizator<TInput, TOutput>
    {
        readonly Func<TInput, TOutput> _generator;
        readonly ConcurrentDictionary<TInput, TOutput> _cache;

        public Memoizator(Func<TInput, TOutput> generator)
        {
            this._generator = generator;
            this._cache = new ConcurrentDictionary<TInput, TOutput>();
        }

        public Memoizator(Func<TInput, TOutput> generator, IEqualityComparer<TInput> comparer)
        {
            this._generator = generator;
            this._cache = new ConcurrentDictionary<TInput, TOutput>(comparer);
        }

        public TOutput GetValue(TInput input)
        {
            return _cache.GetOrAdd(input, _generator);
        }
    }
}
