using Microsoft.AspNetCore.Mvc;
using PAW.DataAccess.Repositories;
using PAW.Models;
using PAW.Models.DTO;

namespace PAW.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserRoleController(IUserRoleRepository userRoleRepository) : ControllerBase
    {
        [HttpGet]
        public async Task<IEnumerable<UserRoleDTO>> GetAll()
        {
            var userRoles = await userRoleRepository.ReadAsync() ?? [];
            return userRoles.Select(UserRoleDTO.ConvertFrom);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<UserRoleDTO>> GetById(int id)
        {
            var userRole = await userRoleRepository.FindAsync(id);

            if (userRole == null)
                return NotFound();

            return UserRoleDTO.ConvertFrom(userRole);
        }

        [HttpPost("create")]
        public async Task<bool> CreateNew([FromBody] UserRole userRole)
        {
            return await userRoleRepository.CreateAsync(userRole);
        }

        [HttpPost]
        public async Task<bool> Save([FromBody] IEnumerable<UserRole> userRoles)
        {
            foreach (var ur in userRoles)
            {
                if (ur.Id > 0)
                    await userRoleRepository.UpdateAsync(ur);
                else
                    await userRoleRepository.CreateAsync(ur);
            }

            return true;
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            var userRole = await userRoleRepository.FindAsync(id);

            if (userRole == null)
                return NotFound();

            var result = await userRoleRepository.DeleteAsync(userRole);

            return Ok(result);
        }
    }
}