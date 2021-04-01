using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CurrencyConverter.Api.Entity;

namespace CurrencyConverter.Api.Tests
{
    public class TargetAmountConverterTests
    {
        private ITargetAmountConverter _targetAmountConverter;
        Dictionary<string, double> _rates;

        [SetUp]
        public void Setup()
        {
            _targetAmountConverter = new TargetAmountConverter();
            _rates = new Dictionary<string, double>
            {
                { "CAD", 1.734793273},
                { "HKD", 10.69 },
                { "ISK", 173.80 },
                { "PHP", 66.77 },
                { "DKK", 8.72 }
            };
        }

        [TestCase("CAD", 10, 17.34793273)]
        [TestCase("HKD", 10, 106.9)]
        [TestCase("ISK", 5, 869)]
        [TestCase("PHP", 5, 333.85)]
        [TestCase("DKK", 5, 43.6)]
        public async Task Given_Currency_And_Amount_Then_CalculateAmount_Should_Returns_Targetvalue(string target, double amount, double expectedAmount)
        {
            
            //ACT
            var currency = await _targetAmountConverter.CalculateAmount(_rates, target, amount);
            
            //Assert
            Assert.AreEqual(Math.Round(expectedAmount,2), Math.Round(currency.Amount, 2));
        }
     
        [TestCase("AUD", 5, 0)]
        [TestCase("NZD", 10, 0)]
        public async Task Given_Currency_And_Amount_Then_CalculateAmount_Should_Returns_ZERO_When_Currency_Is_NotAvailable(string target, double amount, double expectedAmount)
        { 
            //ACT
            var currency = await _targetAmountConverter.CalculateAmount(_rates, target, amount);

            //Assert
            Assert.AreEqual(expectedAmount, currency.Amount);
        }

        [Test]
        public async Task Given_Currency_And_Amount_Then_CalculateAmount_Should_Returns_Currency()
        {
            //ACT
            var currency = await _targetAmountConverter.CalculateAmount(_rates, "CAD", 5);
            //Assert
            Assert.That(currency, Is.TypeOf<Currency>());
        }
    }
}
