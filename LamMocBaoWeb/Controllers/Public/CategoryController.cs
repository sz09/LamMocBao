using AutoMapper;
using LamMocBaoWeb.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Services.Services.Interfaces;
using System.Threading.Tasks;

namespace LamMocBaoWeb.Controllers.Public
{
    [Route("danh-muc")]
    public class CategoryController : BaseController
    {
        private readonly ICategoryService _categoryService;
        public CategoryController(IMapper mapper, ICategoryService categoryService) : base(mapper)
        {
            _categoryService = categoryService;
        }


        [Route("{linkName}")]
        public async Task<IActionResult> PageForCategory(string linkName)
        {
            var productType = await _categoryService.FindAsync(d => d.LinkName == linkName);
            if (productType == null)
            {
                return NotFound();
            }

            return View("Public/PageForCategory", _mapper.Map<CategoryViewModel>(productType));
        }
    }
}
