using AutoMapper;
using Humanizer;
using LamMocBaoWeb.Models;
using LamMocBaoWeb.Models.Appointment;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Services.Caching;
using Services.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LamMocBaoWeb.Controllers.Public
{
    [Route("dat-lich")]
    public class AppointmentController : BaseController
    {
        private readonly IAppointmentService _appointmentService;
        private readonly InMemoryCache _cache;
        private readonly IWebHostEnvironment _environment;
        public AppointmentController(IMapper mapper, IAppointmentService appointmentService, InMemoryCache cache, IWebHostEnvironment environment) : base(mapper)
        {
            _appointmentService = appointmentService;
            _cache = cache;
            _environment = environment;
        }

        [Route("")]
        [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Any)]
        public IActionResult Index()
        {
            return View("Public/Index");
        }

        [Route("hoan-thanh")]
        [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Any)]
        public IActionResult ThankYou()
        {
            return View("Public/ThankYou");
        }

        [Route("")]
        [HttpPost]
        public async Task<IActionResult> MakeAAppointment(AppointmentModel model)
        {
            DateTime? birthDay = null;
            var addressList = GetAddressList();

            var province = addressList.FirstOrDefault(d => d.Id == model.Province);
            var district = province.Districts.FirstOrDefault(d => d.Id == model.District);
            var ward = district.Wards.FirstOrDefault(d => d.Id == model.Ward);
            if (!string.IsNullOrEmpty(model.BirthDayDate))
            {
                var combinedDateTime = $"{model.BirthDayDate} {model.BirthDayTime ?? "00:00"}";
                birthDay = DateTime.Parse(combinedDateTime);
                birthDay = DateTime.SpecifyKind(birthDay.Value, DateTimeKind.Unspecified);
            }
            var app = _mapper.Map<Shared.Models.Appointment>(model);
            app.BirthDay = birthDay;
            app.Address = model.NumberAndStreet;
            app.Province = province.Name;
            app.District = district.Name;
            app.Ward = ward.Name;
            await _appointmentService.AddAsync(app);
            return RedirectToAction(nameof(ThankYou));
        }
        private List<Province> GetAddressList()
        {
            return _cache.TryGet1Async(nameof(Province).Pluralize(), () =>
            {
                var path = Path.Combine(_environment.WebRootPath, "lib", "vn-address.json");
                using (StreamReader r = new StreamReader(path))
                {
                    string json = r.ReadToEnd();
                    return JsonConvert.DeserializeObject<List<Province>>(json);
                }
            });
        }
    }
}
