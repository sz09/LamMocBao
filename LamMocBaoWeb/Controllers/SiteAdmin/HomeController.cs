using AutoMapper;
using LamMocBaoWeb.Controllers.Admin;
using Microsoft.AspNetCore.Mvc;

namespace LamMocBaoWeb.Controllers.SiteAdmin
{
    [Route("admin")]
    public class HomeController : AdminController
    {
        public HomeController(IMapper mapper) : base(mapper)
        {
        }

        [Route("")]
        public IActionResult Index()
        {
            return RedirectToAction("ProductAdmin", "Product");
        }
    }
}
