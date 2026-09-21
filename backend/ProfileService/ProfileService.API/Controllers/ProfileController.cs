using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProfileService.Business.Dtos.Requests;
using ProfileService.Business.Dtos.Responses;
using ProfileService.Business.Services.Interfaces;

namespace ProfileService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService _profileService;

        public ProfileController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _profileService.GetAllProfilesAsync();
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _profileService.GetProfileByIdAsync(id);
            if (result == null) return NotFound("Không tìm thấy hồ sơ người dùng.");
            return Ok(result);
        }

        [HttpGet("by-user/{userId:guid}")]
        public async Task<IActionResult> GetByUserId(Guid userId)
        {
            var result = await _profileService.GetProfileByUserIdAsync(userId);
            if (result == null) return NotFound("Không tìm thấy hồ sơ cho UserId này.");
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProfileRequest request)
        {
            var result = await _profileService.CreateProfileAsync(request);
            return Ok(result);
        }

        [HttpPut("by-user/{userId:guid}")]
        public async Task<IActionResult> Update(Guid userId, [FromBody] ProfileRequest request)
        {
            var result = await _profileService.UpdateProfileAsync(userId, request);
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var success = await _profileService.DeleteProfileAsync(id);
            if (!success) return NotFound("Không tìm thấy hồ sơ người dùng.");
            return Ok("Xóa hồ sơ thành công.");
        }
    }
}