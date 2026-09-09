using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProfileService.Business.Dtos;
using ProfileService.Business.Services.Interfaces;

namespace ProfileService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PhongBanController : ControllerBase
    {
        private readonly IPhongBanService _phongBanService;

        public PhongBanController(IPhongBanService phongBanService)
        {
            _phongBanService = phongBanService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _phongBanService.GetAllPhongBansAsync();
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _phongBanService.GetPhongBanByIdAsync(id);
            if (result == null) return NotFound("Không tìm thấy phòng ban.");
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePhongBanRequest request)
        {
            var result = await _phongBanService.CreatePhongBanAsync(request);
            return Ok(result);
        }
    }
}
