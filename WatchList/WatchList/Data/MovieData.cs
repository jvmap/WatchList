using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WatchList.Data
{
    public class MovieData : IEnumerable<(string, string)>
    {
        private readonly IDictionary<string, string> _data;

        public MovieData(IDictionary<string, string> data)
        {
            this._data = data;
        }

        public MovieData(IEnumerable<(string, string)> data)
        {
            this._data = new Dictionary<string, string>(
                data.Select(((string k, string v) t) => new KeyValuePair<string, string>(t.k, t.v)));
        }

        public string this[string property]
        {
            get
            {
                return _data[property];
            }
        }

        public IEnumerator<(string, string)> GetEnumerator() => PrivateGetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => PrivateGetEnumerator();

        private IEnumerator<(string, string)> PrivateGetEnumerator()
        {
            return new EnumeratorWrapper(_data.GetEnumerator());
        }

        class EnumeratorWrapper : IEnumerator<(string, string)>
        {
            private readonly IEnumerator<KeyValuePair<string, string>> _input;

            public EnumeratorWrapper(IEnumerator<KeyValuePair<string, string>> input)
            {
                this._input = input;
            }

            public (string, string) Current => PrivateCurrent;

            object IEnumerator.Current => PrivateCurrent;

            private (string, string) PrivateCurrent => (_input.Current.Key, _input.Current.Value);

            public void Dispose()
            {
                _input.Dispose();
            }

            public bool MoveNext()
            {
                return _input.MoveNext();
            }

            public void Reset()
            {
                _input.Reset();
            }
        }
    }
}
