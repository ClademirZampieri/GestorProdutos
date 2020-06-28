using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace NDD.Gerenciamento.Geral.Clients.Http
{
    public class StandardHttpClient : IHttpClient
    {
        private readonly HttpClient _client;

        public StandardHttpClient()
        {
            _client = new HttpClient();
        }

        #region Private Methods

        public async Task<HttpResponseMessage> SendAsync<T>(HttpMethod method, string uri, T item, string authorizationToken = null, string requestId = null, string authorizationMethod = "Bearer")
        {
            // a new StringContent must be created for each retry
            // as it is disposed after each call

            var requestMessage = new HttpRequestMessage(method, uri);

            requestMessage.Content = new StringContent(JsonConvert.SerializeObject(item), System.Text.Encoding.UTF8, "application/json");

            if (authorizationToken != null)
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue(authorizationMethod, authorizationToken);
            }

            if (requestId != null)
            {
                requestMessage.Headers.Add("x-requestid", requestId);
            }

            var response = await _client.SendAsync(requestMessage);

            // raise exception if HttpResponseCode 500
            // needed for circuit breaker to track fails

            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                throw new HttpRequestException();
            }

            return response;
        }

        #endregion

        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
        {
            var response = await _client.SendAsync(request);
            return response;
        }


        public async Task<HttpResponseMessage> GetAsync(string uri, string authorizationToken = null, string authorizationMethod = "Bearer")
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            if (authorizationToken != null)
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue(authorizationMethod, authorizationToken);
            }

            var response = await _client.SendAsync(requestMessage);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            // raise exception if HttpResponseCode 500
            // needed for circuit breaker to track fails

            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                throw new HttpRequestException();
            }

            return response;

        }
        public async Task<TReturn> GetAsync<TReturn>(string uri, string authorizationToken = null, string authorizationMethod = "Bearer") where TReturn : class, new()
        {
            var response = await GetAsync(uri, authorizationToken, authorizationMethod);
            return JsonConvert.DeserializeObject<TReturn>(await response.Content.ReadAsStringAsync());
        }


        public async Task<HttpResponseMessage> PostAsync<TSend>(string uri, TSend item, string authorizationToken = null, string requestId = null, string authorizationMethod = "Bearer")
        {
            return await SendAsync(HttpMethod.Post, uri, item, authorizationToken, requestId, authorizationMethod);
        }
        public async Task<TReturn> PostAsync<TSend, TReturn>(string uri, TSend item, string authorizationToken = null, string requestId = null, string authorizationMethod = "Bearer")
        {
            var response = await SendAsync(HttpMethod.Post, uri, item, authorizationToken, requestId, authorizationMethod);
            return JsonConvert.DeserializeObject<TReturn>(await response.Content.ReadAsStringAsync());
        }

        public async Task<HttpResponseMessage> PutAsync<TSend>(string uri, TSend item, string authorizationToken = null, string requestId = null, string authorizationMethod = "Bearer")
        {
            return await SendAsync(HttpMethod.Put, uri, item, authorizationToken, requestId, authorizationMethod);
        }
        public async Task<TReturn> PutAsync<TSend, TReturn>(string uri, TSend item, string authorizationToken = null, string requestId = null, string authorizationMethod = "Bearer")
        {
            var response = await SendAsync(HttpMethod.Put, uri, item, authorizationToken, requestId, authorizationMethod);
            return JsonConvert.DeserializeObject<TReturn>(await response.Content.ReadAsStringAsync());
        }

        public async Task<TReturn> DeleteAsync<TSend, TReturn>(string uri, TSend item, string authorizationToken = null, string requestId = null, string authorizationMethod = "Bearer")
        {
            var response = await SendAsync(HttpMethod.Delete, uri, item, authorizationToken, requestId, authorizationMethod);
            return JsonConvert.DeserializeObject<TReturn>(await response.Content.ReadAsStringAsync());
        }

        public async Task<HttpResponseMessage> DeleteAsync(string uri, string authorizationToken = null, string requestId = null, string authorizationMethod = "Bearer")
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Delete, uri);

            if (authorizationToken != null)
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue(authorizationMethod, authorizationToken);
            }

            if (requestId != null)
            {
                requestMessage.Headers.Add("x-requestid", requestId);
            }

            return await _client.SendAsync(requestMessage);
        }
    }
}

