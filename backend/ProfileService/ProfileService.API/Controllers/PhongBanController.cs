using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProfileService.Business.Dtos.Requests;
using ProfileService.Business.Dtos.Responses;
using ProfileService.Business.Services.Interfaces;
using Shared.Contracts.Exceptions;
using Shared.Contracts.Responses;

namespace ProfileService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [Produces("application/json")]
    public class PhongBanController : ControllerBase
    {
        private readonly IPhongBanService _phongBanService;

        public PhongBanController(IPhongBanService phongBanService)
        {
            _phongBanService = phongBanService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<PhongBanDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApiResponse<PagedResult<PhongBanDto>>>> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _phongBanService.GetPhongBansAsync(page, pageSize);
            return Ok(ApiResponse<PagedResult<PhongBanDto>>.SuccessResult(result));
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<PhongBanDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<PhongBanDto>>> GetById(Guid id)
        {
            var result = await _phongBanService.GetPhongBanByIdAsync(id);
            if (result == null) throw new NotFoundException("Không tìm thấy phòng ban.");
            return Ok(ApiResponse<PhongBanDto>.SuccessResult(result));
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<PhongBanDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ApiResponse<PhongBanDto>>> Create([FromBody] PhongBanRequest request)
        {
            var result = await _phongBanService.CreatePhongBanAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<PhongBanDto>.SuccessResult(result));
        }
    }
}
