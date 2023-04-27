using AutoMapper;
using LamMocBaoWeb.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Services.Services.Interfaces;
using System.Threading.Tasks;

namespace LamMocBaoWeb.Controllers.Public
{
    [Route("vat-pham/ngu-hanh")]
    public class ProductTypeController : BaseController
    {
        private readonly IProductTypeService _productTypeService;
        public ProductTypeController(IMapper mapper,  IProductTypeService productTypeService) : base(mapper)
        {
            _productTypeService = productTypeService;
        }

        [Route("{linkName}")]
        public async Task<IActionResult> PageForProductType(string linkName) 
        {
            var productType = await _productTypeService.FindAsync(d => d.LinkName == linkName);
            if (productType == null)
            {
                return NotFound();
            }

            return View("Public/PageForProductType", _mapper.Map<ProductTypeViewModel>(productType));
        }
    }
}
