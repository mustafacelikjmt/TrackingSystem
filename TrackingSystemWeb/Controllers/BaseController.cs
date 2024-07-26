using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace TrackingSystemWeb.Controllers
{
    public class BaseController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            var actionName = context.RouteData.Values["action"].ToString();
            ViewData["Title"] = $"{actionName}";
        }
    }
}