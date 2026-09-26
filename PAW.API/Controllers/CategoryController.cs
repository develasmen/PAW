using Microsoft.AspNetCore.Mvc;
using PAW.DataAccess.Repositories;
using PAW.Models;
using PAW.Models.DTO;

namespace PAW.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CategoryController(
        ILogger<CategoryController> logger,
        ICategoryRepository categoryRepository) : ControllerBase
    {
        [HttpGet(Name = "GetCategories")]
        public async Task<IEnumerable<CategoryDTO>> GetAll()
        {
            var categories = await categoryRepository.ReadAsync() ?? [];

            return categories.Select(CategoryDTO.ConvertFrom);
        }

        [HttpGet("{id:int}", Name = "GetCategoryById")]
        public async Task<ActionResult<CategoryDTO>> GetById(int id)
        {
            var category = await categoryRepository.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            return CategoryDTO.ConvertFrom(category);
        }

        [HttpPost]
        public async Task<bool> Save([FromBody] IEnumerable<Category> categories)
        {
            foreach (var c in categories)
            {
                if (c.CategoryId > 0)
                    await categoryRepository.UpdateAsync(c);
                else
                    await categoryRepository.CreateAsync(c);
            }

            return true;
        }

        [HttpDelete]
        public async Task<bool> Delete(Category category)
        {
            return await categoryRepository.DeleteAsync(category);
        }
    }
}