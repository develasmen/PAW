using Microsoft.AspNetCore.Mvc;
using PAW.DataAccess.Repositories;
using PAW.Models;
using PAW.Models.DTO;

namespace PAW.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InventoryController(
        ILogger<InventoryController> logger,
        IInventoryRepository inventoryRepository) : ControllerBase
    {
        [HttpGet(Name = "GetInventories")]
        public async Task<IEnumerable<InventoryDTO>> GetAll()
        {
            var inventories = await inventoryRepository.ReadAsync() ?? [];

            return inventories.Select(InventoryDTO.ConvertFrom);
        }

        [HttpGet("{id:int}", Name = "GetInventoryById")]
        public async Task<ActionResult<InventoryDTO>> GetById(int id)
        {
            var inventory = await inventoryRepository.FindAsync(id);

            if (inventory == null)
            {
                return NotFound();
            }

            return InventoryDTO.ConvertFrom(inventory);
        }

        [HttpPost]
        public async Task<bool> Save([FromBody] IEnumerable<Inventory> inventories)
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

        [HttpDelete]
        public async Task<bool> Delete(Inventory inventory)
        {
            return await inventoryRepository.DeleteAsync(inventory);
        }
    }
}