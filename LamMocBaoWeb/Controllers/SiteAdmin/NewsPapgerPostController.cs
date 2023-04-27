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
    [Route("admin/bao-chi")]
    public class NewsPaperPostController : AdminController
    {
        public INewsPaperPostService _newsPaperPostService;
        public NewsPaperPostController(INewsPaperPostService newsPaperPostService, IMapper mapper) : base(mapper)
        {
            _newsPaperPostService = newsPaperPostService;
        }

        [Route("")]
        [HttpGet]
        [ResponseCache(NoStore = true, Duration = 0)]
        public IActionResult NewsPaperPostAdmin(SearchQuery<Shared.Models.NewsPaperPost> searchQuery = null)
        {
            var searchResult = _newsPaperPostService.Search(searchQuery);
            ViewBag.Page = searchQuery.Page;
            ViewBag.SearchText = searchQuery.Search;
            return View("Admin/NewsPaperPostAdmin", ResultListView<Shared.Models.NewsPaperPost>.From(searchResult, searchQuery.PageSize, _mapper));
        }

        [HttpGet]
        [Route("create")]
        public ActionResult NewsPaperPostAdminCreate()
        {
            ViewBag.IsCreate = true;
            return View("Admin/NewsPaperPostAdminCreate", new CreateNewsPaperPostModel());
        }

        [Route("create")]
        [HttpPost]
        public async Task<IActionResult> NewsPaperPostAdminCreate(CreateNewsPaperPostModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Admin/NewsPaperPostAdminCreate", model);
            }

            var newsPaperPost = _mapper.Map<Shared.Models.NewsPaperPost>(model);

            await _newsPaperPostService.AddAsync(newsPaperPost);
            return RedirectToAction("NewsPaperPostAdmin");
        }

        [Route("edit/{id}")]
        [HttpGet]
        public async Task<ActionResult> NewsPaperPostAdminEdit(Guid id)
        {
            var product = await _newsPaperPostService.LoadAsync(id);
            return View("Admin/NewsPaperPostAdminEdit", _mapper.Map<EditNewsPaperPostViewModel>(product));
        }

        [Route("update")]
        [HttpPost]
        public async Task<IActionResult> NewsPaperPostAdminUpdate(UpdateNewsPaperPostModel model)
        {
            if (!ModelState.IsValid)
            {
                var p1 = _mapper.Map<EditNewsPaperPostViewModel>(model);
                return View("Admin/NewsPaperPostAdminEdit", p1);
            }

            var product = _mapper.Map<Shared.Models.NewsPaperPost>(model);
            await _newsPaperPostService.UpdateAsync(product.Id, (d) =>
            {
                d.Hint = model.Hint;
                d.Link = model.Link;
                d.SequenceNumber = model.SequenceNumber;
                d.UploadedImageId = model.UploadedImageId.Value;
            });

            return RedirectToAction("NewsPaperPostAdmin");
        }

        [HttpDelete]
        [Route("delete/{id}")]
        public async Task<ActionResult> NewsPaperPostDelete(Guid id)
        {
            await _newsPaperPostService.DeleteAsync(id);
            return Success();
        }
    }
}
