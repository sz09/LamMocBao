using LamMocBaoWeb.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Services;
using Services.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LamMocBaoWeb.Controllers
{
    [AllowAnonymous]
    [Route("files")]
    public class FileController : Controller
    {
        private readonly IFileServices _fileServices;
        private readonly IUploadedImageService _uploadedImageService;
        public FileController(IFileServices fileServices, IUploadedImageService uploadedImageService)
        {
            _fileServices = fileServices;
            _uploadedImageService = uploadedImageService;
        }

        [HttpPost]
        [Route("uploads")]
        public async Task<JsonResult> Uploads(Guid entityId)
        {
            var files = Request.Form.Files.ToList();
            var uploadImageResult = await _fileServices.UploadAsync(entityId, files);
            List<UploadedFileViewModel> newItems = new List<UploadedFileViewModel>();
            foreach (var item in uploadImageResult)
            {
                var id = await _uploadedImageService.AddAsync(new Shared.Models.UploadedImage
                {
                    EntityId = entityId,
                    Url = item.OriginalUri,
                    UrlPreview = item.SecondaryUri
                });

                newItems.Add(new UploadedFileViewModel { Url = item.OriginalUri, Id = id});
            }

            return new JsonResult(newItems);
        }

        [HttpPost]
        [Route("upload")]
        public async Task<JsonResult> Upload(Guid entityId)
        {
            var files = Request.Form.Files.ToList();
            if (files.Any())
            {
                var uploadImageResult = await _fileServices.UploadAsync(entityId, new List<Microsoft.AspNetCore.Http.IFormFile> { files.First() });
                var model = new Shared.Models.UploadedImage
                {
                    EntityId = entityId,
                    Url = uploadImageResult.First().OriginalUri,
                    UrlPreview = uploadImageResult.First().SecondaryUri
                };
                var id = await _uploadedImageService.AddAsync(model);
                return new JsonResult(new
                {
                    Url = model.Url,
                    Id = id
                });
            }

            return new JsonResult(new { });
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
