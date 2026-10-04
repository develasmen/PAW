using Microsoft.AspNetCore.Mvc;
using PAW.DataAccess.Repositories;
using PAW.Models;
using PAW.Models.DTO;

namespace PAW.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ComponentController(
        IComponentRepository componentRepository) : ControllerBase
    {
        [HttpGet]
        public async Task<IEnumerable<ComponentDTO>> GetAll()
        {
            var components = await componentRepository.ReadAsync() ?? [];

            return components.Select(ComponentDTO.ConvertFrom);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ComponentDTO>> GetById(int id)
        {
            var component = await componentRepository.FindAsync(id);

            if (component == null)
                return NotFound();

            return ComponentDTO.ConvertFrom(component);
        }

        [HttpPost("create")]
        public async Task<bool> CreateNew([FromBody] Component component)
        {
            return await componentRepository.CreateAsync(component);
        }

        [HttpPost]
        public async Task<bool> Save(
            [FromBody] IEnumerable<Component> components)
        {
            foreach (var component in components)
            {
                if (component.Id > 0)
                    await componentRepository.UpdateAsync(component);
                else
                    await componentRepository.CreateAsync(component);
            }

            return true;
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            var component = await componentRepository.FindAsync(id);

            if (component == null)
                return NotFound();

            var result =
                await componentRepository.DeleteAsync(component);

            return Ok(result);
        }
    }
}