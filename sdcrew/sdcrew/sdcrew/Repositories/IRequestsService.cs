using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace sdcrew.Repositories
{
    public interface IRequestsService
    {
        Task<TResult> GetAsync<TResult>(string uri, string token = "");

        Task<string> GetAsyncJsonResult(string uri, string token = "");

        Task<string> GetJsonResult(string uri, string token = "");

        Task<string> PostAsync<TResult>(string uri, TResult data, string token = "");

        Task<string> PostAsync<TRequest, TResult>(string uri, TRequest data, string token = "");

        Task<bool> Post_Custom(dynamic obj, string url);

        Task<TResult> PutAsync<TResult>(string uri, TResult data, string token = "");

        Task<TResult> PutAsync<TRequest, TResult>(string uri, TRequest data, string token = "");
    }
}
