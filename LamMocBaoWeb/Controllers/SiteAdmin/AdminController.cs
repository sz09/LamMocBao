using AutoMapper;
using LamMocBaoWeb.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace LamMocBaoWeb.Controllers.Admin
{
    [LMBAuthorized]
    public class AdminController : BaseController
    {
        public AdminController( IMapper mapper) : base(mapper)
        {
        }

        protected ActionResult RedirectToGrid()
        {
            if (HttpContext.Request.Headers.TryGetValue("Referer", out StringValues headerReferer))
            {
                return base.Redirect(headerReferer);
            }

            return base.Redirect(HttpContext.Request.Host.Host);
        }
    }
}
