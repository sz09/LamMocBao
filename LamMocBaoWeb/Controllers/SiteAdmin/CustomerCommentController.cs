using AutoMapper;
using LamMocBaoWeb.Models;
using LamMocBaoWeb.Utilities;
using LamMocBaoWeb.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Services.Services.Interfaces;
using Shared.Utilities;
using System;
using System.Threading.Tasks;

namespace LamMocBaoWeb.Controllers.Admin
{
    [Route("admin/cam-nhan-khach-hang")]
    public class CustomerCommentController : AdminController
    {
        public ICustomerCommentService _newsPaperPostService;
        public CustomerCommentController(ICustomerCommentService newsPaperPostService, IMapper mapper) : base(mapper)
        {
            _newsPaperPostService = newsPaperPostService;
        }

        [Route("")]
        [HttpGet]
        [ResponseCache(NoStore = true, Duration = 0)]
        public IActionResult CustomerCommentAdmin(SearchQuery<Shared.Models.CustomerComment> searchQuery = null)
        {
            var searchResult = _newsPaperPostService.Search(searchQuery);
            ViewBag.Page = searchQuery.Page;
            ViewBag.SearchText = searchQuery.Search;
            return View("Admin/CustomerCommentAdmin", ResultListView<Shared.Models.CustomerComment>.From(searchResult, searchQuery.PageSize, _mapper));
        }

        [HttpGet]
        [Route("create")]
        public ActionResult CustomerCommentAdminCreate()
        {
            ViewBag.IsCreate = true;
            return View("Admin/CustomerCommentAdminCreate", new CreateCustomerCommentModel());
        }

        [Route("create")]
        [HttpPost]
        public async Task<IActionResult> CustomerCommentAdminCreate(CreateCustomerCommentModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Admin/CustomerCommentAdminCreate", model);
            }

            var newsPaperPost = _mapper.Map<Shared.Models.CustomerComment>(model);

            await _newsPaperPostService.AddAsync(newsPaperPost);
            return RedirectToAction("CustomerCommentAdmin");
        }

        [Route("edit/{id}")]
        [HttpGet]
        public async Task<ActionResult> CustomerCommentAdminEdit(Guid id)
        {
            var product = await _newsPaperPostService.LoadAsync(id);
            return View("Admin/CustomerCommentAdminEdit", _mapper.Map<EditCustomerCommentViewModel>(product));
        }

        [Route("update")]
        [HttpPost]
        public async Task<IActionResult> CustomerCommentAdminUpdate(UpdateCustomerCommentModel model)
        {
            if (!ModelState.IsValid)
            {
                var p1 = _mapper.Map<EditCustomerCommentViewModel>(model);
                return View("Admin/CustomerCommentAdminEdit", p1);
            }

            var product = _mapper.Map<Shared.Models.CustomerComment>(model);
            await _newsPaperPostService.UpdateAsync(product.Id, (d) =>
            {
                d.Hint = model.Hint;
                d.SequenceNumber = model.SequenceNumber;
                d.UploadedImageId = model.UploadedImageId.Value;
            });

            return RedirectToAction("CustomerCommentAdmin");
        }

        [HttpDelete]
        [Route("delete/{id}")]
        public async Task<ActionResult> CustomerCommentDelete(Guid id)
        {
            await _newsPaperPostService.DeleteAsync(id);
            return Success();
        }
    }
}
