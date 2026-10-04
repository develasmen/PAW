using Microsoft.AspNetCore.Mvc;
using PAW.DataAccess.Repositories;
using PAW.Models;
using PAW.Models.DTO;

namespace PAW.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserActionController(IUserActionRepository userActionRepository) : ControllerBase
    {
        [HttpGet]
        public async Task<IEnumerable<UserActionDTO>> GetAll()
        {
            var userActions = await userActionRepository.ReadAsync() ?? [];
            return userActions.Select(UserActionDTO.ConvertFrom);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<UserActionDTO>> GetById(int id)
        {
            var userAction = await userActionRepository.FindAsync(id);

            if (userAction == null)
                return NotFound();

            return UserActionDTO.ConvertFrom(userAction);
        }

        [HttpPost]
        public async Task<bool> Save([FromBody] IEnumerable<UserAction> userActions)
        {
            foreach (var ua in userActions)
            {
                if (ua.Id > 0)
                    await userActionRepository.UpdateAsync(ua);
                else
                    await userActionRepository.CreateAsync(ua);
            }

            return true;
        }

        [HttpPost("create")]
        public async Task<bool> CreateNew([FromBody] UserAction userAction)
        {
            return await userActionRepository.CreateAsync(userAction);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            var userAction = await userActionRepository.FindAsync(id);

            if (userAction == null)
                return NotFound();

            var result = await userActionRepository.DeleteAsync(userAction);

            return Ok(result);
        }


    }
}