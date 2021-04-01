using System.Net.Http;
using System.Threading.Tasks;

namespace CurrencyConverter.Api.Client
{
    public interface IHttpClientWrapper
    {
        Task<HttpResponseMessage> GetAsync(string url);
    }
}
