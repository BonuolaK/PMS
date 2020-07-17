using PMS.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PMS.Shared.HttpService
{
    public class HttpService : IHttpService
    {
        private HttpClient _httpClient;
        public Task<TResult> DeleteAsync<TResult>(string requestUri)
        {
            throw new NotImplementedException();
        }

        public Task<TResult> GetAsync<TResult>(string requestUri)
        {
          return  _httpClient.GetAsync<TResult>(requestUri);
        }

        public Task<TResult> PostAsJsonAsync<TModel, TResult>(string requestUri, TModel model)
        {
            throw new NotImplementedException();
        }

        public Task<TResult> PutAsync<TModel, TResult>(string requestUri, TModel model)
        {
            throw new NotImplementedException();
        }
    }
}
