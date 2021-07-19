using System;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace Billing.Client
{
    public abstract class ClientBase
    {
        public Func<Task<string>>? RetrieveAuthorizationToken { get; set; }

        // Called by implementing swagger client classes
        protected async Task<HttpRequestMessage> CreateHttpRequestMessageAsync(CancellationToken cancellationToken)
        {
            var msg = new HttpRequestMessage();

            if (RetrieveAuthorizationToken != null)
            {
                var token = await RetrieveAuthorizationToken();
                msg.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
            return msg;
        }

        public async Task<T> GetAsync<T>(string url)
        {
            var httpClient = GetHttpClient();

            return JsonConvert.DeserializeObject<T>(
                await httpClient.GetStringAsync(url)
            )!;
        }

        public async Task PostAsync(string url, object content)
        {
            var httpClient = GetHttpClient();

            await httpClient.PostAsync(url, new StringContent(JsonConvert.SerializeObject(content)));
        }

        public async Task<T> PostAsync<T>(string url, object content)
        {
            var httpClient = GetHttpClient();

            var response = await httpClient.PostAsync(url, new StringContent(JsonConvert.SerializeObject(content)));

            return JsonConvert.DeserializeObject<T>(
                await response.Content.ReadAsStringAsync()
            )!;
        }

        public async Task PutAsync(string url, object content)
        {
            var httpClient = GetHttpClient();

            await httpClient.PutAsync(url, new StringContent(JsonConvert.SerializeObject(content)));
        }

        public async Task<T> PutAsync<T>(string url, object content)
        {
            var httpClient = GetHttpClient();

            var response = await httpClient.PutAsync(url, new StringContent(JsonConvert.SerializeObject(content)));

            return JsonConvert.DeserializeObject<T>(
                await response.Content.ReadAsStringAsync()
            )!;
        }

        public async Task DeleteAsync(string url)
        {
            var httpClient = GetHttpClient();

            await httpClient.DeleteAsync(url);
        }

        private HttpClient GetHttpClient()
        {
            return (HttpClient)this.GetType()
                .GetField("_httpClient", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)!
                .GetValue(this)!;
        }
    }
}