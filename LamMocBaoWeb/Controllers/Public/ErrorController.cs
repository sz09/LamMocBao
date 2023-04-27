using Microsoft.AspNetCore.Mvc;

namespace LamMocBaoWeb.Controllers.Public
{
    [Route("error")]
    public class ErrorController : Controller
    {
        [Route("")]
        [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Any)]
        public IActionResult Index()
        {
            return View();
        }

        [Route("not-found")]
        [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Any)]
        public IActionResult NotFound()
        {
            return View();
        }
    }
}
