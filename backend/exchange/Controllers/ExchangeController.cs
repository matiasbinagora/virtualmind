using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace exchange.Controllers
{
    [Route("api/[controller]")]
    public class ExchangeController : Controller
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        private readonly string dolarServiceUrl = "http://www.bancoprovincia.com.ar/Principal/Dolar";
        private readonly string realServiceUrl = string.Empty;
        private readonly string dolarCanadianServiceUrl = string.Empty;

        // GET: api/exchange
        [HttpGet("{currency}"), Route("getCurrency")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ExchangeRate(string currency)
        {
            currency = currency.ToLower();

            // check valid request
            if (string.IsNullOrEmpty(currency) || (!currency.Contains("dolar") && !currency.Contains("real") && !currency.Contains("canadian")))
            {
                return BadRequest();
            }

            if (currency.Contains("dolar"))
            {
                return await GetDolarExchangeRateFromExternal();
            }
            if (currency.Contains("canadian"))
            {
                return await GetDolarCanadianExchangeRateFromExternal();
            }

            return await GetRealExchangeRateFromExternal();

        }

        // GET api/values/5
        //[HttpGet("{id}")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        /// <summary>
        /// Gets the actual exchange from external API.
        /// </summary>
        /// <returns>The exchange rate from external.</returns>
        /// <param name="url">URL of the external service.</param>
        private async Task<IActionResult> GetExchangeRateFromExternal(string url)
        {
            using (var response = await _httpClient.GetAsync(url))
            {
                if (!response.IsSuccessStatusCode)
                    return StatusCode((int)response.StatusCode);

                var responseContent = await response.Content.ReadAsStringAsync();
                var deserializedResponse = JsonConvert.DeserializeObject<List<string>>(responseContent);

                return Ok(deserializedResponse);
            }
        }

        /// <summary>
        /// Gets the real exchange rate from external.
        /// </summary>
        /// <returns>The real exchange rate from external.</returns>
        private async Task<IActionResult> GetRealExchangeRateFromExternal()
        {
            if (string.IsNullOrEmpty(realServiceUrl))
            {
                var response = await GetExchangeRateFromExternal(dolarServiceUrl);

                // take the sold result and make the division to 4 and return.
            }

            return await GetExchangeRateFromExternal(realServiceUrl);
        }

        /// <summary>
        /// Gets the actual exchange from external API from Province Bank.
        /// </summary>
        /// <returns>The dolar exchange rate from external.</returns>
        private async Task<IActionResult> GetDolarExchangeRateFromExternal()
        {
            return await GetExchangeRateFromExternal(dolarServiceUrl);
        }

        /// <summary>
        /// Gets the dolar canadian exchange rate.
        /// </summary>
        /// <returns>The dolar canadian exchange rate.</returns>
        private async Task<IActionResult> GetDolarCanadianExchangeRateFromExternal()
        {
            return await GetExchangeRateFromExternal(dolarCanadianServiceUrl);
        }
    }
}