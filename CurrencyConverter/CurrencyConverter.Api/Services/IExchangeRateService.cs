using CurrencyConverter.Api.Entity;
using System.Threading.Tasks;

namespace CurrencyConverter.Api.Services
{
    public interface IExchangeRateService
    {
        Task<Exchange> InvokeClientAsync(string baseCurrency);
    }
}
