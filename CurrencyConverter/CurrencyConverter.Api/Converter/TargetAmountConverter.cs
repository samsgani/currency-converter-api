using CurrencyConverter.Api.Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CurrencyConverter.Api
{
    public class TargetAmountConverter : ITargetAmountConverter
    {
        public async Task<Currency> CalculateAmount(Dictionary<string, double> rates, string target, double amount)
        {
            double targetValue = 0;

            if (rates != null && rates.ContainsKey(target))
            {
                targetValue = rates[target] * amount;
            }

            return await Task.FromResult(new Currency { Amount = targetValue, Type = target });
        }
    }
}
