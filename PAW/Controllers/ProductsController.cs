using Microsoft.AspNetCore.Mvc;
using PAW.data.models;
using PAW.Repositories;

namespace PAW.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductRepository _productRepository;

    public ProductsController(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    // GET: api/products
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> Get()
    {
        var products = await _productRepository.ReadAsync();
        return Ok(products);
    }

    // GET: api/products/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> Get(int id)
    {
        var product = await _productRepository.FindAsync(id);
        if (product == null) return NotFound();
        return Ok(product);
    }

    // POST: api/products
    [HttpPost]
    public async Task<ActionResult> Post([FromBody] Product product)
    {
        var created = await _productRepository.CreateAsync(product);
        if (!created) return BadRequest();
        return CreatedAtAction(nameof(Get), new { id = product.ProductId }, product);
    }

    // PUT: api/products/5
    [HttpPut("{id}")]
    public async Task<ActionResult> Put(int id, [FromBody] Product product)
    {
        if (id != product.ProductId) return BadRequest();
        var updated = await _productRepository.UpdateAsync(product);
        if (!updated) return BadRequest();
        return NoContent();
    }

    // DELETE: api/products/5
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var product = await _productRepository.FindAsync(id);
        if (product == null) return NotFound();
        var deleted = await _productRepository.DeleteAsync(product);
        if (!deleted) return BadRequest();
        return NoContent();
    }
}