using Microsoft.AspNetCore.Mvc;
using PAW.DataAccess.Repositories;
using PAW.Models;
using PAW.Models.DTO;

namespace PAW.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ComponentController(
        ILogger<ComponentController> logger,
        IComponentRepository componentRepository) : ControllerBase
    {
        [HttpGet(Name = "GetComponents")]
        public async Task<IEnumerable<ComponentDTO>> GetAll()
        {
            var components = await componentRepository.ReadAsync() ?? [];

            return components.Select(ComponentDTO.ConvertFrom);
        }

        [HttpGet("{id}", Name = "GetComponentById")]
        public async Task<ActionResult<ComponentDTO>> GetById(decimal id)
        {
            var component =
                await componentRepository.FindByIdAsync(id);

            if (component == null)
            {
                return NotFound();
            }

            return ComponentDTO.ConvertFrom(component);
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

        [HttpDelete]
        public async Task<bool> Delete(Component component)
        {
            return await componentRepository.DeleteAsync(component);
        }
    }
}