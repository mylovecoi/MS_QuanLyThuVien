using System.Threading.Tasks;
using IdentityService.API.Authorization;
using IdentityService.DataAccess.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RoleController : ControllerBase
    {
        private readonly IRoleRepository _roleRepository;

        public RoleController(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        [HttpGet]
        [HasPermission("roles.read")]
        public async Task<IActionResult> GetAllRoles()
        {
            var roles = await _roleRepository.GetAllWithPermissionsAsync();
            return Ok(roles);
        }
    }
}
