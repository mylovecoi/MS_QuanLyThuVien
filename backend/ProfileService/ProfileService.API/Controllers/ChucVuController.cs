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
    public class ChucVuController : ControllerBase
    {
        private readonly IChucVuService _chucVuService;

        public ChucVuController(IChucVuService chucVuService)
        {
            _chucVuService = chucVuService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<ChucVuDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApiResponse<PagedResult<ChucVuDto>>>> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _chucVuService.GetChucVusAsync(page, pageSize);
            return Ok(ApiResponse<PagedResult<ChucVuDto>>.SuccessResult(result));
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<ChucVuDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<ChucVuDto>>> GetById(Guid id)
        {
            var result = await _chucVuService.GetChucVuByIdAsync(id);
            if (result == null) throw new NotFoundException("Không tìm thấy chức vụ.");
            return Ok(ApiResponse<ChucVuDto>.SuccessResult(result));
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<ChucVuDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ApiResponse<ChucVuDto>>> Create([FromBody] ChucVuRequest request)
        {
            var result = await _chucVuService.CreateChucVuAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<ChucVuDto>.SuccessResult(result));
        }
    }
}
