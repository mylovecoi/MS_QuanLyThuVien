using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProfileService.Business.Dtos;
using ProfileService.Business.Services.Interfaces;

namespace ProfileService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChucVuController : ControllerBase
    {
        private readonly IChucVuService _chucVuService;

        public ChucVuController(IChucVuService chucVuService)
        {
            _chucVuService = chucVuService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _chucVuService.GetAllChucVusAsync();
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _chucVuService.GetChucVuByIdAsync(id);
            if (result == null) return NotFound("Không tìm thấy chức vụ.");
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateChucVuRequest request)
        {
            var result = await _chucVuService.CreateChucVuAsync(request);
            return Ok(result);
        }
    }
}
