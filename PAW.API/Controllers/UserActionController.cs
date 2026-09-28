using Microsoft.AspNetCore.Mvc;
using PAW.DataAccess.Repositories;
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
    }
}

//UserAction esta configurada como HasNoKey(), no hay llave real para buscar/actualizar/eliminar un registro puntual.

