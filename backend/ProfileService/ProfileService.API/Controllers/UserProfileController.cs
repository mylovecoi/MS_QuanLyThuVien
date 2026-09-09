using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProfileService.Business.Dtos;
using ProfileService.Business.Services.Interfaces;

namespace ProfileService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserProfileController : ControllerBase
    {
        private readonly IUserProfileService _userProfileService;

        public UserProfileController(IUserProfileService userProfileService)
        {
            _userProfileService = userProfileService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _userProfileService.GetAllProfilesAsync();
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _userProfileService.GetProfileByIdAsync(id);
            if (result == null) return NotFound("Không tìm thấy hồ sơ người dùng.");
            return Ok(result);
        }

        [HttpGet("by-user/{userId:guid}")]
        public async Task<IActionResult> GetByUserId(Guid userId)
        {
            var result = await _userProfileService.GetProfileByUserIdAsync(userId);
            if (result == null) return NotFound("Không tìm thấy hồ sơ cho UserId này.");
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserProfileRequest request)
        {
            var result = await _userProfileService.CreateProfileAsync(request);
            return Ok(result);
        }

        [HttpPut("by-user/{userId:guid}")]
        public async Task<IActionResult> Update(Guid userId, [FromBody] UpdateUserProfileRequest request)
        {
            var result = await _userProfileService.UpdateProfileAsync(userId, request);
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var success = await _userProfileService.DeleteProfileAsync(id);
            if (!success) return NotFound("Không tìm thấy hồ sơ người dùng.");
            return Ok("Xóa hồ sơ thành công.");
        }
    }
}
