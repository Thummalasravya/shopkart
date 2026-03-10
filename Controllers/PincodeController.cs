using Microsoft.AspNetCore.Mvc;
using System.Net.Http;

namespace ECommerceAPI.Controllers
{
    [ApiController]
    [Route("api/pincode")]
    public class PincodeController : ControllerBase
    {
        private readonly HttpClient _http;

        public PincodeController(HttpClient http)
        {
            _http = http;
        }

        [HttpGet("{pin}")]
        public async Task<IActionResult> Lookup(string pin)
        {
            var res = await _http.GetAsync(
                $"https://api.postalpincode.in/pincode/{pin}");

            var content = await res.Content.ReadAsStringAsync();

            return Content(content, "application/json");
        }
    }
}
