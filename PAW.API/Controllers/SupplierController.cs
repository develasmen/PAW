using Microsoft.AspNetCore.Mvc;
using PAW.DataAccess.Repositories;
using PAW.Models;
using PAW.Models.DTO;

namespace PAW.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SupplierController(
        ILogger<SupplierController> logger,
        ISupplierRepository supplierRepository) : ControllerBase
    {
        [HttpGet(Name = "GetSuppliers")]
        public async Task<IEnumerable<SupplierDTO>> GetAll()
        {
            var suppliers = await supplierRepository.ReadAsync() ?? [];
            return suppliers.Select(SupplierDTO.ConvertFrom);
        }

        [HttpGet("{id:int}", Name = "GetSupplierById")]
        public async Task<ActionResult<SupplierDTO>> GetById(int id)
        {
            var supplier = await supplierRepository.FindAsync(id);

            if (supplier == null)
            {
                return NotFound();
            }

            return SupplierDTO.ConvertFrom(supplier);
        }

        [HttpPost]
        public async Task<bool> Save([FromBody] IEnumerable<Supplier> suppliers)
        {
            foreach (var s in suppliers)
            {
                if (s.SupplierId > 0)
                    await supplierRepository.UpdateAsync(s);
                else
                    await supplierRepository.CreateAsync(s);
            }

            return true;
        }

        [HttpDelete]
        public async Task<bool> Delete(Supplier supplier)
        {
            return await supplierRepository.DeleteAsync(supplier);
        }
    }
}