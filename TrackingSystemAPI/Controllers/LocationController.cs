using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TrackingSystemAPI.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public LocationController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [HttpGet]
        public async Task<IActionResult> GetLocationHistory()
        {
            var requestUrl = "http://localhost:8080/api/locations";
            var response = await _httpClient.GetAsync(requestUrl);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return Ok(content);
            }
            return Ok("LocationController");
        }
    }
}