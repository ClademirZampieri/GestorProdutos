using System.Net.Http;
using System.Threading.Tasks;

namespace NDD.Gerenciamento.Geral.Clients.Http
{
    public interface IHttpClient
    {
        Task<HttpResponseMessage> SendAsync<T>(HttpMethod method, string uri, T item, string authorizationToken = null, string requestId = null, string authorizationMethod = "Bearer");
        Task<HttpResponseMessage> GetAsync(string uri, string authorizationToken = null, string authorizationMethod = "Bearer");
        Task<TReturn> GetAsync<TReturn>(string uri, string authorizationToken = null, string authorizationMethod = "Bearer") where TReturn : class, new();

        Task<HttpResponseMessage> PostAsync<TSend>(string uri, TSend item, string authorizationToken = null, string requestId = null, string authorizationMethod = "Bearer");
        Task<TReturn> PostAsync<TSend, TReturn>(string uri, TSend item, string authorizationToken = null, string requestId = null, string authorizationMethod = "Bearer");

        Task<TReturn> DeleteAsync<TSend, TReturn>(string uri, TSend item, string authorizationToken = null, string requestId = null, string authorizationMethod = "Bearer");
        Task<HttpResponseMessage> DeleteAsync(string uri, string authorizationToken = null, string requestId = null, string authorizationMethod = "Bearer");

        Task<HttpResponseMessage> PutAsync<TSend>(string uri, TSend item, string authorizationToken = null, string requestId = null, string authorizationMethod = "Bearer");
        Task<TReturn> PutAsync<TSend, TReturn>(string uri, TSend item, string authorizationToken = null, string requestId = null, string authorizationMethod = "Bearer");
    }
}
