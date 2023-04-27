using AutoMapper;
using LamMocBaoWeb.Controllers.Admin;
using LamMocBaoWeb.Utilities;
using LamMocBaoWeb.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Services.Services.Interfaces;
using Shared.Utilities;
using System;
using System.Threading.Tasks;

namespace LamMocBaoWeb.Controllers.SiteAdmin
{
    [Route("admin/dat-lich")]
    public class AppointmentController : AdminController
    {
        private readonly IAppointmentService _appointmentService;
        public AppointmentController(IMapper mapper, IAppointmentService appointmentService) : base(mapper)
        {
            _appointmentService = appointmentService;
        }

        [Route("")]
        [HttpGet]
        public IActionResult AppointmentAdmin(SearchQuery<Shared.Models.Appointment> searchQuery = null)
        {
            var searchResult = _appointmentService.Search(searchQuery);
            ViewBag.Page = searchQuery.Page;
            ViewBag.SearchText = searchQuery.Search;
            return View("Admin/AppointmentAdmin", ResultListView<Shared.Models.Appointment>.From(searchResult, searchQuery.PageSize, _mapper));
        }

        public IActionResult Index()
        {
            return View();
        }


        [Route("edit/{id}")]
        [HttpGet]
        public async Task<ActionResult> AppointmentAdminView(Guid id)
        {
            var order = await _appointmentService.LoadAsync(id);
            return View("Admin/AppointmentAdminView", _mapper.Map<ViewAppointmentViewModel>(order));
        }

    }
}
