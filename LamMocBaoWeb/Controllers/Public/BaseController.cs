using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace LamMocBaoWeb.Controllers
{
    [IgnoreAntiforgeryToken(Order = 2000)]
    [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Any)]
    public class BaseController : Controller
    {
        protected readonly IMapper _mapper;
        public BaseController(IMapper mapper)
        {
            _mapper = mapper;
        }

        protected JsonResult Success(object data = null)
        {
            return new JsonResult(
                new { 
                    Success = true,
                    Data = data
                });
        }
        
        protected JsonResult Failed(object data = null)
        {
            return new JsonResult(
                new { 
                    Success = false,
                    Data = data
                });
        }
        
        protected IActionResult LMBNotFound()
        {
            return RedirectToAction("NotFound", "Error");
        }

        protected JsonResult InvalidModel()
        {
            return new JsonResult(new { 
                Success = false,
                ErrorMessage = "Model is invalid"
            });
        }
    }
}
