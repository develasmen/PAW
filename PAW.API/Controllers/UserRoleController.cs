using Microsoft.AspNetCore.Mvc;
using PAW.DataAccess.Repositories;
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
    }
}