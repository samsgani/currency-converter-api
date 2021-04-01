using NUnit.Framework;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using CurrencyConverter.Api.Controllers;
using Moq;
using System.Threading.Tasks;
using CurrencyConverter.Api.Services;
using CurrencyConverter.Api.Entity;
using System;

namespace CurrencyConverter.Api.Tests
{
    public class ExchangeControllerTests
    {
        private ExchangeController _exchangeController;
        private Mock<IExchangeRateService> _externalService;
        private Mock<ITargetAmountConverter> _targetAmountConverter;

        [SetUp]
        public void Setup()
        {
            _externalService = new Mock<IExchangeRateService>();
            _targetAmountConverter = new Mock<ITargetAmountConverter>();
            _exchangeController = new ExchangeController(_externalService.Object, _targetAmountConverter.Object);
        }

        [Test]
        public async Task Given_Valid_Values_Then_GetApiEndpoint_Should_Returns_Converted_Currency_And_OkResult()
        {
            // Arrange
            var source = "GBP";
            var target = "CAD";
            var amount = 6;

            var currency = new Currency { Amount = 10.38, Type = "CAD" };

            var exchange = new Exchange() { Rates = new Dictionary<string, double>() { { "CAD", 1.73 }, { "HKD", 10.69 } } };

            _externalService.Setup(service => service.InvokeClientAsync(source)).ReturnsAsync(exchange);

            _targetAmountConverter.Setup(convertor => convertor.CalculateAmount(new Dictionary<string, double>(), "CAD", 0)).ReturnsAsync(currency);

            _exchangeController = new ExchangeController(_externalService.Object, _targetAmountConverter.Object);

            // Act
            var result = await _exchangeController.Get(source, target, amount) as OkObjectResult;

            //Assert
            Assert.That(result, Is.TypeOf<OkObjectResult>());

            Assert.AreEqual(result.StatusCode, StatusCodes.Status200OK);

            _externalService.Verify(e => e.InvokeClientAsync(It.IsAny<string>()), Times.Once());

            _targetAmountConverter.Verify(c => c.CalculateAmount(It.IsAny<Dictionary<string, double>>(), It.IsAny<string>(), It.IsAny<double>()), Times.Once());
        }

        [Test]
        public async Task Given_Valid_Values_Then_GetApiEndpoint_Should_Returns_Converted_Currency()
        {
            // Arrange
            var source = "GBP";
            var target = "CAD";
            var amount = 6;
            var targetAmount = 10.38;

            var currency = new Currency { Amount = targetAmount, Type = target };

            var exchange = new Exchange() { Rates = new Dictionary<string, double>() { { "CAD", 1.73 }, { "HKD", 10.69 } } };

            _externalService.Setup(service => service.InvokeClientAsync(source)).ReturnsAsync(exchange);

            _targetAmountConverter.Setup(convertor => convertor.CalculateAmount(exchange.Rates, target, amount)).ReturnsAsync(currency);

            _exchangeController = new ExchangeController(_externalService.Object, _targetAmountConverter.Object);

            // Act
            var result = await _exchangeController.Get(source, target, amount) as OkObjectResult;

            //Assert
            Assert.That(result.Value, Is.TypeOf<Currency>());

            Assert.AreEqual(Math.Round(((Currency)result.Value).Amount, 2), targetAmount);

            Assert.AreEqual(((Currency)result.Value).Type, target);
        }

        [TestCase("", "CAD", 50)]
        [TestCase("GBP", "", 50)]
        [TestCase("GBP", "CAD", 0)]
        public async Task GetApiEndpoint_Should_Returns_BadRequest_When_Input_Parameters_Are_Not_Valid(string source, string target, double amount)
        {
            // Arrange

            // Act
            var result = await _exchangeController.Get(source, target, amount) as BadRequestObjectResult;

            //Assert

            Assert.That(result, Is.TypeOf<BadRequestObjectResult>());

            _externalService.Verify(e => e.InvokeClientAsync(It.IsAny<string>()), Times.Never());

            _targetAmountConverter.Verify(c => c.CalculateAmount(It.IsAny<Dictionary<string, double>>(), It.IsAny<string>(), It.IsAny<double>()), Times.Never());

            Assert.AreEqual(result.StatusCode, StatusCodes.Status400BadRequest);

        }

        [Test]
        public async Task GetApiEndpoint_Should_Returns_NotfoundResult_When_CurrencyExchangeAPI_Returns_Null()
        {
            // Arrange
            var source = "GBP";
            var target = "CAD";
            var amount = 6;

            // Act
            var result = await _exchangeController.Get(source, target, amount) as NotFoundObjectResult;

            //Assert

            Assert.That(result, Is.TypeOf<NotFoundObjectResult>());

            Assert.AreEqual(result.StatusCode, StatusCodes.Status404NotFound);

            _targetAmountConverter.Verify(c => c.CalculateAmount(It.IsAny<Dictionary<string, double>>(), It.IsAny<string>(), It.IsAny<double>()), Times.Never());

            Assert.AreEqual(result.Value, $"unable to find the Currency Conversion for {source}");
        }

        [Test]
        public async Task GetApiEndpoint_Should_Returns_NotfoundResult_When_Input_Parameters_Are_Not_Matched_With_CurrencyExchangeAPI()
        {
            var source = "GBP";
            var target = "CAD";
            var amount = 6;

            var exchange = new Exchange() { Rates = new Dictionary<string, double>() { } };

            _externalService.Setup(service => service.InvokeClientAsync(source)).ReturnsAsync(exchange);

            // Act
            var result = await _exchangeController.Get(source, target, amount) as NotFoundObjectResult;

            //Assert

            Assert.That(result, Is.TypeOf<NotFoundObjectResult>());

            Assert.AreEqual(result.StatusCode, StatusCodes.Status404NotFound);

            _targetAmountConverter.Verify(c => c.CalculateAmount(It.IsAny<Dictionary<string, double>>(), It.IsAny<string>(), It.IsAny<double>()), Times.Never());

            Assert.AreEqual(result.Value, $"unable to find the Currency Conversion for {source}");
        }

        [Test]
        public async Task GetApiEndpoint_Should_Throws_an_Exception_When_an_Exception_From_External_CurrencyExchangeAPI()
        {
            // Arrange
            var source = "GBP";
            var target = "CAD";
            var amount = 6;
            _externalService.Setup(service => service.InvokeClientAsync(source)).ThrowsAsync(new Exception());

            // Act
            var result = await _exchangeController.Get(source, target, amount) as ObjectResult;
            //Assert

            _targetAmountConverter.Verify(c => c.CalculateAmount(It.IsAny<Dictionary<string, double>>(), It.IsAny<string>(), It.IsAny<double>()), Times.Never());

            Assert.AreEqual(result.StatusCode, StatusCodes.Status500InternalServerError);

        }
    }
}
