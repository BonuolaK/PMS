using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PMS.Shared.HttpService
{
    public interface IHttpService
    {
        Task<TResult> GetAsync<TResult>(string requestUri);

        Task<TResult> DeleteAsync<TResult>(string requestUri);

        Task<TResult> PostAsJsonAsync<TModel, TResult>(string requestUri, TModel model);

        Task<TResult> PutAsync<TModel, TResult>(string requestUri, TModel model);
    }
}
