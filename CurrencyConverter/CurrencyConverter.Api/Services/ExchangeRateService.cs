using System.Threading.Tasks;
using Newtonsoft.Json;
using CurrencyConverter.Api.Client;
using CurrencyConverter.Api.Configuration;
using CurrencyConverter.Api.Entity;

namespace CurrencyConverter.Api.Services
{
    /// <summary>
    /// To call the exchangeratesapi
    /// </summary>
    public class ExchangeRateService : IExchangeRateService
    {
        private readonly IHttpClientWrapper _clientWrapper;
        private readonly IConfigProvider _configuration;
        public ExchangeRateService(IHttpClientWrapper clientWrapper, IConfigProvider configuration)
        {
            _clientWrapper = clientWrapper;
            _configuration = configuration;
        }

        public async Task<Exchange> InvokeClientAsync(string baseCurrency)
        {   
            var response = await _clientWrapper.GetAsync(string.Format(_configuration.ExchangeApiUrl, baseCurrency));
            if (!response.IsSuccessStatusCode)
            {
                //need to log the failure message and status code for monitoring and alerting
                return null;
            }
            else
            {
                var result = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Exchange>(result);
            }
        }
    }  
}
