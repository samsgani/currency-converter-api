using CurrencyConverter.Api.Client;
using CurrencyConverter.Api.Configuration;
using CurrencyConverter.Api.Entity;
using CurrencyConverter.Api.Services;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace CurrencyConverter.Api.Tests
{
    public class ExchangeRateServiceTests
    {
        private string _url;
        private string _baseCurrency;
        private HttpResponseMessage _httpResponseMessage;
        private Mock<IConfigProvider> _configuration;
        private Mock<IHttpClientWrapper> _htpClientWrapper;
        

        [SetUp]
        public void Setup()
        {
            _baseCurrency = "GBP";
            _url = "https://api.exchangeratesapi.io/latest?base={0}";
            _httpResponseMessage = new HttpResponseMessage() { Content = new StringContent(JsonConvert.SerializeObject(new Exchange())) };
            _configuration = new Mock<IConfigProvider>();
            _configuration.Setup(s => s.ExchangeApiUrl).Returns(_url);
            _htpClientWrapper = new Mock<IHttpClientWrapper>();
        }

        [Test]
        public async Task Given_BaseCurrency_Then_InvokeClientAsync_Should_Returns_CurrencyList_When_Success_ExcahangeApi_Call()
        {
            // Arrange

            _httpResponseMessage.StatusCode = System.Net.HttpStatusCode.OK;

            _htpClientWrapper.Setup(t => t.GetAsync(string.Format(_url, _baseCurrency)))
                .ReturnsAsync(_httpResponseMessage);

            var service = new ExchangeRateService(_htpClientWrapper.Object, _configuration.Object);

            // Act
            var result = await service.InvokeClientAsync(_baseCurrency);

            // Assert
            Assert.That(result, Is.TypeOf<Exchange>());
        }

        [Test]
        public async Task Given_BaseCurrency_Then_InvokeClientAsync_Should_Returns_Null_When_Failure_ExcahangeApi_Call()
        {
            // Arrange

            _httpResponseMessage.StatusCode = System.Net.HttpStatusCode.NotFound;

            _htpClientWrapper.Setup(t => t.GetAsync(string.Format(_url, _baseCurrency)))
                .ReturnsAsync(_httpResponseMessage);

            var service = new ExchangeRateService(_htpClientWrapper.Object, _configuration.Object);

            // Act
            var result = await service.InvokeClientAsync(_baseCurrency);

            // Assert
            Assert.AreEqual(result, null);
        }

        [Test]
        public async Task Given_BaseCurrency_Then_InvokeClientAsync_Should_Returns_Null_When_InternalServerError_ExcahangeApi_Call()
        {
            // Arrange

            _httpResponseMessage.StatusCode = System.Net.HttpStatusCode.InternalServerError;

            _htpClientWrapper.Setup(t => t.GetAsync(string.Format(_url, _baseCurrency)))
                .ReturnsAsync(_httpResponseMessage);

            var service = new ExchangeRateService(_htpClientWrapper.Object, _configuration.Object);

            // Act
            var result = await service.InvokeClientAsync(_baseCurrency);

            // Assert
            Assert.AreEqual(result, null);
        }
    }
}
