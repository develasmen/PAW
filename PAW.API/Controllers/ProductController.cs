using Microsoft.AspNetCore.Mvc;
using PAW.DataAccess.Repositories;
using PAW.Models;
using PAW.Models.DTO;

namespace PAW.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController(ILogger<ProductController> logger, IProductRepository productRepository) : ControllerBase
    {
        [HttpGet(Name = "GetProducts")]
        public async Task<IEnumerable<ProductDTO>> GetAll()
        {
            var products = await productRepository.ReadAsync() ?? [];

            return products.Select(ProductDTO.ConvertFrom);
        }

        [HttpGet("{id:int}", Name = "GetProductById")]
        public async Task<ActionResult<ProductDTO>> GetById(int id)
        {
            var product = await productRepository.FindAsync(id);

            if (product == null)
                return NotFound();

            return ProductDTO.ConvertFrom(product);
        }

        /*[HttpPost("filter", Name = "FilterProducts")]
        public async Task<IEnumerable<Product>> Filter(ConditionViewModel condition)
        {
            var predicate = ConditionResolver<Product>.ResolveCondition(condition.Criteria, condition.Property, condition.Value, condition.Start, condition.End);
            var results = await businessProduct.Filter(predicate);
            return results;
        }*/

        [HttpPost]
        public async Task<bool> Save([FromBody] IEnumerable<Product> Products)
        {
            foreach (var p in Products)
            {
                if (p.ProductId > 0)
                    await productRepository.UpdateAsync(p);
                else
                    await productRepository.CreateAsync(p);
            }

            /*Products.ToList().ForEach(async x =>
            {
                if (x.Id > 0)
                    await productRepository.CreateAsync(x);
                else
                    await productRepository.UpdateAsync(x);
            });*/

            return true;
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            var product = await productRepository.FindAsync(id);

            if (product == null)
                return NotFound();

            var result = await productRepository.DeleteAsync(product);

            return Ok(result);
        }
    }
}