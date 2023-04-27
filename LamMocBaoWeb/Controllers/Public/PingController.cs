using Microsoft.AspNetCore.Mvc;
using System;

namespace LamMocBaoWeb.Controllers.Public
{
    [Route("ping")]
    public class PingController : Controller
    {
        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            Console.WriteLine("Ping received");
            return Json(new { Request = true });
        }
    }
}
