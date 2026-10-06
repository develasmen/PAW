using Microsoft.AspNetCore.Mvc;
using PAW.DataAccess.Repositories;
using PAW.Models;
using PAW.Models.DTO;

namespace PAW.API.Controllers;

[ApiController]
[Route("[controller]")]
public class RoleController(IRoleRepository roleRepository) : ControllerBase
{
    [HttpGet]
    public async Task<IEnumerable<RoleDTO>> GetAll()
    {
        var roles = await roleRepository.ReadAsync();
        return roles.Select(RoleDTO.ConvertFrom);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<RoleDTO>> GetById(int id)
    {
        var role = await roleRepository.FindAsync(id);

        if (role == null)
            return NotFound();

        return RoleDTO.ConvertFrom(role);
    }

    [HttpPost]
    public async Task<bool> Save([FromBody] IEnumerable<Role> roles)
    {
        foreach (var role in roles)
        {
            bool saved = role.RoleId > 0
                ? await roleRepository.UpdateAsync(role)
                : await roleRepository.CreateAsync(role);

            if (!saved)
                return false;
        }

        return true;
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<bool>> Delete(int id)
    {
        var role = await roleRepository.FindAsync(id);
        if (role == null)
            return NotFound();

        return Ok(await roleRepository.DeleteAsync(role));
    }
}