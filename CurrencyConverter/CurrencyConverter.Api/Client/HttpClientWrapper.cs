using System.Net.Http;
using System.Threading.Tasks;

namespace CurrencyConverter.Api.Client
{
    public class HttpClientWrapper : IHttpClientWrapper
    {
        private readonly HttpClient HttpClient;
        public HttpClientWrapper(HttpClient httpClient)
        {
            HttpClient = httpClient;
        }
        public async Task<HttpResponseMessage> GetAsync(string url)
        {
            return await HttpClient.GetAsync(url);
        }
    }
}
