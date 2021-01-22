using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using WatchList.Config;

namespace WatchList.Data
{
    public class OmdbMovieRepository : IMovieRepository
    {
        private readonly string _apiKey;

        // MS docs: "HttpClient is intended to be instantiated once and re-used throughout the life of an application."
        private readonly static HttpClient _httpClient = new HttpClient();

        public OmdbMovieRepository(IOptions<OmdbApiConfig> options)
        {
            this._apiKey = options.Value.ApiKey;
        }

        // http://www.omdbapi.com/?apikey=***REMOVED***&t=star%20trek
        public async Task<MovieData> GetMovieByTitleAsync(string title)
        {
            HttpResponseMessage response = await _httpClient.GetAsync(GetRequestUriForTitle(title));
            response.EnsureSuccessStatusCode();
            string content = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);
            if (Found(data))
            {
                // convert to movie object.
                return new MovieData(data.SelectMany(PassStringValues));
            }
            else
            {
                return null;
            }

            static bool Found(Dictionary<string, object> data)
            {
                bool hasResponseField = data.Remove("Response", out object responseValue);

                if (!hasResponseField || responseValue == null)
                    throw new Exception("Required field 'Response' not found in response from OmdbAPI.");

                if (responseValue is string)
                    return bool.Parse((string)responseValue);
                else
                    throw new Exception("Unexpected data type in response from OmdbAPI. Expected string, got: " + responseValue.GetType().ToString());

            }

            static IEnumerable<(string, string)> PassStringValues(KeyValuePair<string, object> input)
            {
                if (input.Value is string)
                {
                    yield return (input.Key, (string)input.Value);
                }
            }
        }

        private string GetRequestUriForTitle(string title)
        {
            return string.Format(
                @"https://www.omdbapi.com/?apikey={0}&t={1}",
                _apiKey,
                HttpUtility.UrlEncode(title));
        }
    }
}
