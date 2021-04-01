using Microsoft.Extensions.Configuration;

namespace CurrencyConverter.Api.Configuration
{
    public class ConfigProvider : IConfigProvider
    {
        private readonly IConfiguration _configuration;
        public ConfigProvider(IConfiguration configuration)
        {
            _configuration = configuration;
            ConfigurationSetup();
        }

        public string ExchangeApiUrl { get; set; }

        private void ConfigurationSetup()   
        {
            ExchangeApiUrl = _configuration.GetValue<string>("ExchangeApi:Url");
        }
    }
}
