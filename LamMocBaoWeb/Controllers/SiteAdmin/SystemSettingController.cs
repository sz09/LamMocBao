using AutoMapper;
using LamMocBaoWeb.Models;
using LamMocBaoWeb.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Services.Services;
using Services.Services.Interfaces;
using Shared.Utilities;
using System.Linq;
using System.Threading.Tasks;

namespace LamMocBaoWeb.Controllers.Admin
{
    [Route("admin/system-setting")]
    public class SystemSettingController : AdminController
    {
        public ISystemSettingService _systemSettingService;
        public IFileServices _fileServices;
        public SystemSettingController(ISystemSettingService systemSettingService, IMapper mapper, IFileServices fileServices) : base(mapper)
        {
            _systemSettingService = systemSettingService;
            _fileServices = fileServices;
        }

        [Route("")]
        [HttpGet]
        [ResponseCache(NoStore = true, Duration = 0)]
        public async Task<IActionResult> SystemSettingAdmin()
        {
            var systemSetting = await _systemSettingService.LoadAsync();
            return View("Admin/SystemSettingAdmin", _mapper.Map<EditSystemSettingViewModel>(systemSetting));
        }

        [Route("")]
        [HttpPost]
        public async Task<IActionResult> SystemSettingAdmin(UpdateSystemSettingModel model)
        {
            if (!ModelState.IsValid)
            {
                var p1 = _mapper.Map<EditSystemSettingViewModel>(model);
                return View("Admin/SystemSettingAdmin", p1);
            }

            var searchResult = _systemSettingService.Search(new SearchQuery<Shared.Models.SystemSetting>());
            await _systemSettingService.UpdateAsync(searchResult.Data.First().Id, (d) =>
            {
                d.Email = model.Email;
                //d.PhoneNumber = model.PhoneNumber;

                d.BankAccount = model.BankAccount;
                d.CardHolderName = model.CardHolderName;
                d.BankName = model.BankName;

                d.MomoPaymentPhoneNumber = model.MomoPaymentPhoneNumber;
                d.MomoPaymentCardHolder = model.MomoPaymentCardHolder;
                d.ContactAddress = model.ContactAddress;
                d.ContactPhoneNumbers = model.ContactPhoneNumbers;
                d.WorkingTime = model.WorkingTime;
                d.GoogleMapFrameUrl = model.GoogleMapFrameUrl;
                d.Facebook = model.Facebook;
                d.Youtube = model.Youtube;
                d.Instagram = model.Instagram;
                d.NumberOfHighlightItems = model.NumberOfHighlightItems;
                d.HighlightItemsInDays = model.HighlightItemsInDays;
            });

            return RedirectToAction(nameof(SystemSettingAdmin));
        }

        [Route("upload-momo-qr")]
        [HttpPost]
        public async Task<IActionResult> UploadMomoQRAsync()
        {
            var files = Request.Form.Files.ToList();
            if (!files.Any())
            {
                return BadRequest();
            }
           var image = await _fileServices.UploadAsync("MomoQRCode", files.First());

            var searchResult = _systemSettingService.Search(new SearchQuery<Shared.Models.SystemSetting>());
            await _systemSettingService.UpdateAsync(searchResult.Data.First().Id, (d) =>
            {
                d.MomoPaymentQRImage = image.OriginalUri;
            });

            return Json(new { Url = image.OriginalUri });
        }
    }
}
