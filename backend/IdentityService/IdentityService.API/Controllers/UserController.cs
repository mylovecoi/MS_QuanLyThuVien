using System;
using System.Threading.Tasks;
using IdentityService.API.Authorization;
using IdentityService.Business.Dtos.Requests;
using IdentityService.Business.Dtos.Responses;
using IdentityService.Business.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Contracts.Responses;

namespace IdentityService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [Produces("application/json")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [HasPermission("users.index")]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<UserDetailResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ApiResponse<PagedResult<UserDetailResponse>>>> GetUsers(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? search = null)
        {
            var result = await _userService.GetUsersAsync(page, pageSize, search);
            return Ok(ApiResponse<PagedResult<UserDetailResponse>>.SuccessResult(result));
        }

        [HttpGet("{id:guid}")]
        [HasPermission("users.index")]
        [ProducesResponseType(typeof(ApiResponse<UserDetailResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<UserDetailResponse>>> GetUserById(Guid id)
        {
            var result = await _userService.GetUserByIdAsync(id);
            return Ok(ApiResponse<UserDetailResponse>.SuccessResult(result));
        }

        [HttpPost]
        [HasPermission("users.create")]
        [ProducesResponseType(typeof(ApiResponse<UserDetailResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ApiResponse<UserDetailResponse>>> CreateUser([FromBody] CreateUserRequest request)
        {
            var result = await _userService.CreateUserAsync(request);
            return StatusCode(StatusCodes.Status201Created, ApiResponse<UserDetailResponse>.SuccessResult(result));
        }

        [HttpPut("{id:guid}")]
        [HasPermission("users.edit")]
        [ProducesResponseType(typeof(ApiResponse<UserDetailResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ApiResponse<UserDetailResponse>>> UpdateUser(Guid id, [FromBody] UpdateUserRequest request)
        {
            var result = await _userService.UpdateUserAsync(id, request);
            return Ok(ApiResponse<UserDetailResponse>.SuccessResult(result));
        }

        [HttpPut("{id:guid}/status")]
        [HasPermission("users.edit")]
        [ProducesResponseType(typeof(ApiResponse<UserDetailResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ApiResponse<UserDetailResponse>>> UpdateStatus(Guid id, [FromBody] ChangeUserStatusRequest request)
        {
            var result = await _userService.UpdateStatusAsync(id, request);
            return Ok(ApiResponse<UserDetailResponse>.SuccessResult(result));
        }

        [HttpPut("{id:guid}/password")]
        [HasPermission("users.edit")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ResetPassword(Guid id, [FromBody] ResetPasswordRequest request)
        {
            await _userService.ResetPasswordAsync(id, request);
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        [HasPermission("users.delete")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            await _userService.DeleteUserAsync(id);
            return NoContent();
        }
    }
}
