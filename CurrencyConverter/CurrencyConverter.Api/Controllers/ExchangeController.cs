using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using CurrencyConverter.Api.Services;

namespace CurrencyConverter.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ExchangeController : ControllerBase
    {
        private IExchangeRateService _exchangeService;
        private ITargetAmountConverter _targetAmountConverter;

        public ExchangeController(IExchangeRateService exchangeService, ITargetAmountConverter targetAmountConverter)
        {
            _exchangeService = exchangeService;
            _targetAmountConverter = targetAmountConverter;
        }

        // GET:<Exchange/Convert?source=GBP&target=CAD&amount=50>
        [HttpGet("Convert")]
        public async Task<IActionResult> Get(string source, string target, double amount)
        {
            try
            {
                ValidateGetRequest(source, target, amount);

                if (ModelState.IsValid)
                {
                    var exchangeList = await _exchangeService.InvokeClientAsync(source);

                    if (exchangeList == null || exchangeList.Rates == null || exchangeList.Rates.Count == 0)
                    {
                        //need to log the notfound message for monitoring and alerting
                        return NotFound($"unable to find the Currency Conversion for {source}");
                    }

                    var currency = await _targetAmountConverter.CalculateAmount(exchangeList.Rates, target, amount);

                    return Ok(currency);
                }
                else
                {
                    //need to log the modelstate error for monitoring and alerting
                    return BadRequest(ModelState);
                }
            }
            catch (Exception ex)
            {
                //need to log the exception for monitoring and alerting
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        private bool ValidateGetRequest(string source, string target, double amount)
        {
            if (String.IsNullOrWhiteSpace(source))
            {
                ModelState.AddModelError("source", "source cannot be empty");
                return false;
            }

            if (String.IsNullOrWhiteSpace(target))
            {
                ModelState.AddModelError("target", "target cannot be empty");
                return false;
            }

            if (amount <= 0)
            {
                ModelState.AddModelError("amount", "amount should be greater than ZERO");
                return false;
            }

            return true;
        }
    }
}
