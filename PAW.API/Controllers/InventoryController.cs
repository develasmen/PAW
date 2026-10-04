using Microsoft.AspNetCore.Mvc;
using PAW.DataAccess.Repositories;
using PAW.Models;
using PAW.Models.DTO;

namespace PAW.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InventoryController(
        IInventoryRepository inventoryRepository) : ControllerBase
    {
        [HttpGet]
        public async Task<IEnumerable<InventoryDTO>> GetAll()
        {
            var inventories =
                await inventoryRepository.ReadAsync() ?? [];

            return inventories.Select(InventoryDTO.ConvertFrom);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<InventoryDTO>> GetById(int id)
        {
            var inventory =
                await inventoryRepository.FindWithProductsAsync(id);

            if (inventory == null)
                return NotFound();

            return InventoryDTO.ConvertFrom(inventory);
        }

        [HttpPost("create")]
        public async Task<bool> CreateNew(
            [FromBody] Inventory inventory)
        {
            return await inventoryRepository.CreateAsync(inventory);
        }

        [HttpPost]
        public async Task<bool> Save(
            [FromBody] IEnumerable<Inventory> inventories)
        {
            foreach (var inventory in inventories)
            {
                if (inventory.InventoryId > 0)
                    await inventoryRepository.UpdateAsync(inventory);
                else
                    await inventoryRepository.CreateAsync(inventory);
            }

            return true;
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            var inventory =
                await inventoryRepository.FindAsync(id);

            if (inventory == null)
                return NotFound();

            var result =
                await inventoryRepository.DeleteAsync(inventory);

            return Ok(result);
        }
    }
}