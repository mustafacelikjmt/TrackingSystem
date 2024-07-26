using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using TrackingSystemWeb.Filters;
using TrackingSystemWeb.Models;
using TrackingSystemWeb.ViewModel;

namespace TrackingSystemWeb.Controllers
{
    public class AccountController : BaseController
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AccountController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [RedirectIfAuthenticated]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var client = _httpClientFactory.CreateClient();
                var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
                var response = await client.PostAsync("http://localhost:5278/api/Account/login", content);
                if (response.IsSuccessStatusCode)
                {
                    var jsonData = await response.Content.ReadAsStringAsync();
                    var tokenModel = JsonSerializer.Deserialize<JwtTokenModel>(jsonData, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    });
                    if (tokenModel != null)
                    {
                        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                        var token = tokenHandler.ReadJwtToken(tokenModel.Token);
                        var claims = token.Claims.ToList();
                        //Unix timestamp ten DateTime dönüşümü.
                        var expireUnixTimestamp = long.Parse(token.Claims.FirstOrDefault(x => x.Type == "exp")?.Value);
                        var expireDate = DateTimeOffset.FromUnixTimeSeconds(expireUnixTimestamp).UtcDateTime;
                        tokenModel.ExpireDate = expireDate;
                        if (tokenModel.Token != null)
                        {
                            claims.Add(new Claim("JWToken", tokenModel.Token));
                            var claimsIdentity = new ClaimsIdentity(claims, JwtBearerDefaults.AuthenticationScheme);
                            var authProps = new AuthenticationProperties
                            {
                                ExpiresUtc = tokenModel.ExpireDate,
                                IsPersistent = true
                            };
                            await HttpContext.SignInAsync(JwtBearerDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProps);
                            return RedirectToAction("Index", "Home");
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Email Veya Parola Hatalı.");
                }
            }
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login");
        }

        [RedirectIfAuthenticated]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ForgotPassword(ForgotPasswordViewModel model)
        {
            return View(model);
        }

        [RedirectIfAuthenticated]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var client = _httpClientFactory.CreateClient();
                var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
                var response = await client.PostAsync("http://localhost:5278/api/Account/register", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();
                    return RedirectToAction("Login");
                }
            }
            return View(model);
        }
    }
}