using Microsoft.AspNetCore.Mvc;

namespace TrackingSystemWeb.Controllers
{
    public class ErrorController : BaseController
    {
        public IActionResult Error401()
        {
            return View();
        }

        public IActionResult Error404()
        {
            return View();
        }

        public IActionResult Error500()
        {
            return View();
        }
    }
}