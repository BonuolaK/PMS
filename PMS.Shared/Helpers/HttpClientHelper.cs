using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;

namespace PMS.Shared.Helpers
{

    public static class HttpClientHelper
    {
        public static async Task<TResult> GetAsync<TResult>(this HttpClient client, string requestUri)
        {
            var response = await client.GetAsync(requestUri);

            response.EnsureSuccessStatusCode();

            var str = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TResult>(str);
        }
    }
}
