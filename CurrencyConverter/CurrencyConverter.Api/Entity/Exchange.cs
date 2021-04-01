using System.Collections.Generic;

namespace CurrencyConverter.Api.Entity
{
    public class Exchange
    {
        public string Base { get; set; }

        public string Date { get; set; }

        public Dictionary<string, double> Rates { get; set; }
    }
}
