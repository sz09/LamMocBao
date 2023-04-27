using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Services.Services;
using Services.Services.Interfaces;
using Services.Utiltities;
using Shared.Models;
using System;
using System.Threading.Tasks;

namespace LamMocBaoWeb.Controllers.Admin
{
    [Route("admin/high-light")]
    public class HighlightController : AdminController
    {
        public readonly IHighlightService _highlightService;
        private readonly IServiceConfig _serviceConfig;
        public HighlightController(IHighlightService highlightService, IMapper mapper, IServiceConfig serviceConfig) : base(mapper)
        {
            _highlightService = highlightService;
            _serviceConfig = serviceConfig;
        }

        [HttpPost]
        [Route("auto")]
        public async Task<IActionResult> AutoHightlight(Guid entityId, EntityType entityType)
        {
            var from = DateTime.Now.StartOfDate().AddSeconds(1);
            var to = DateTime.Now.AddDays(_serviceConfig.AutoHighlightItemsInDays).EndOfDate().AddSeconds(-10);
            await _highlightService.MakeHighlightAsync(entityId, entityType, from, to);

            return Success();
        }
    }
}
