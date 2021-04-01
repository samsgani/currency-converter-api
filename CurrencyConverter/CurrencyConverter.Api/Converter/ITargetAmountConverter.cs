using CurrencyConverter.Api.Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CurrencyConverter.Api
{
    public interface ITargetAmountConverter
    {
        Task<Currency> CalculateAmount(Dictionary<string, double> rates, string target, double amount);
    }
}
