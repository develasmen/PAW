using Microsoft.AspNetCore.Mvc;
using PAW.DataAccess.Repositories;
using PAW.Models;
using PAW.Models.DTO;

namespace PAW.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController(
        ILogger<UserController> logger,
        IUserRepository userRepository) : ControllerBase
    {
        [HttpGet(Name = "GetUsers")]
        public async Task<IEnumerable<UserDTO>> GetAll()
        {
            var users = await userRepository.ReadAsync() ?? [];

            return users.Select(UserDTO.ConvertFrom);
        }

        [HttpGet("{id:int}", Name = "GetUserById")]
        public async Task<ActionResult<UserDTO>> GetById(int id)
        {
            var user = await userRepository.FindAsync(id);

            if (user == null)
                return NotFound();

            return UserDTO.ConvertFrom(user);
        }

        [HttpPost]
        public async Task<ActionResult<bool>> Save([FromBody] IEnumerable<UserDTO> users)
        {
            foreach (var dto in users)
            {
                if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Email))
                    return BadRequest("Username and email are required.");

                if (dto.UserId > 0)
                {
                    var existing = await userRepository.FindAsync(dto.UserId);
                    if (existing == null)
                        return NotFound();

                    existing.Username = dto.Username;
                    existing.Email = dto.Email;
                    existing.IsActive = dto.IsActive;
                    existing.RoleId = dto.RoleId;
                    existing.ModifiedBy = dto.ModifiedBy;
                    existing.LastModifiedBy = dto.ModifiedBy;
                    existing.LastModified = DateTime.UtcNow;

                    if (!await userRepository.UpdateAsync(existing))
                        return Ok(false);
                }
                else
                {
                    var user = new User
                    {
                        Username = dto.Username,
                        Email = dto.Email,
                        IsActive = dto.IsActive,
                        RoleId = dto.RoleId,
                        ModifiedBy = dto.ModifiedBy,
                        LastModifiedBy = dto.ModifiedBy,
                        CreatedAt = DateTime.UtcNow,
                        LastModified = DateTime.UtcNow
                    };

                    if (!await userRepository.CreateAsync(user))
                        return Ok(false);
                }
            }

            return Ok(true);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            var user = await userRepository.FindAsync(id);
            if (user == null)
                return NotFound();

            return Ok(await userRepository.DeleteAsync(user));
        }
    }
}