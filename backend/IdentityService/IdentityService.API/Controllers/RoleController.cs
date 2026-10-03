using System;
using System.Collections.Generic;
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
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet]
        [HasPermission("roles.read")]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<RoleResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ApiResponse<PagedResult<RoleResponse>>>> GetAllRoles(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _roleService.GetRolesAsync(page, pageSize);
            return Ok(ApiResponse<PagedResult<RoleResponse>>.SuccessResult(result));
        }

        // Khai báo trước {id:guid} để rõ ý định; route này không phải Guid nên
        // vẫn không xung đột với {id:guid} nhờ ràng buộc kiểu tham số.
        [HttpGet("permissions")]
        [HasPermission("roles.read")]
        [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<PermissionResponse>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<IReadOnlyList<PermissionResponse>>>> GetAllPermissions()
        {
            var result = await _roleService.GetAllPermissionsAsync();
            return Ok(ApiResponse<IReadOnlyList<PermissionResponse>>.SuccessResult(result));
        }

        [HttpGet("{id:guid}")]
        [HasPermission("roles.read")]
        [ProducesResponseType(typeof(ApiResponse<RoleResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<RoleResponse>>> GetRoleById(Guid id)
        {
            var result = await _roleService.GetRoleByIdAsync(id);
            return Ok(ApiResponse<RoleResponse>.SuccessResult(result));
        }

        [HttpPost]
        [HasPermission("roles.write")]
        [ProducesResponseType(typeof(ApiResponse<RoleResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ApiResponse<RoleResponse>>> CreateRole([FromBody] RoleRequest request)
        {
            var result = await _roleService.CreateRoleAsync(request);
            return StatusCode(StatusCodes.Status201Created, ApiResponse<RoleResponse>.SuccessResult(result));
        }

        [HttpPut("{id:guid}")]
        [HasPermission("roles.write")]
        [ProducesResponseType(typeof(ApiResponse<RoleResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ApiResponse<RoleResponse>>> UpdateRole(Guid id, [FromBody] RoleRequest request)
        {
            var result = await _roleService.UpdateRoleAsync(id, request);
            return Ok(ApiResponse<RoleResponse>.SuccessResult(result));
        }

        [HttpDelete("{id:guid}")]
        [HasPermission("roles.write")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> DeleteRole(Guid id)
        {
            await _roleService.DeleteRoleAsync(id);
            return NoContent();
        }
    }
}