using AutoMapper;
using Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;
using TrackingSystemWeb.ViewModel;

namespace TrackingSystemWeb.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory, IMapper mapper, IConfiguration configuration = null)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _mapper = mapper;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            var token = User.Claims.FirstOrDefault(x => x.Type == "JWToken")?.Value;
            if (token == null)
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }

        //[HttpGet("api/GetMapsApiKey")]
        public IActionResult GetMapsApiKey()
        {
            var mapsApiKey = _configuration["MapsApiKey"];
            Console.WriteLine(mapsApiKey);
            return Json(new { apiKey = mapsApiKey });
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Tables()
        {
            return View();
        }

        public IActionResult Charts()
        {
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> GetLocationHistory()
        {
            var locations = new List<LocationModel>();
            var token = User.Claims.FirstOrDefault(x => x.Type == "JWToken")?.Value;

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                using (var response = await httpClient.GetAsync("http://localhost:5278/api/Location"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    locations = JsonSerializer.Deserialize<List<LocationModel>>(apiResponse);
                }
            }
            var locationViewModels = _mapper.Map<List<LocationsViewModel>>(locations.ToList());
            return Json(locationViewModels);
        }
    }
}