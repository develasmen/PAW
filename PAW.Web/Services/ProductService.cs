using System.Text.Json;
using APW.Architecture;
using PAW.Architecture.Providers;
using PAW.Models;
using PAW.Models.DTO;

namespace PAW.Web.Services
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDTO>> GetProductsAsync();
        Task<ProductDTO?> GetProductByIdAsync(int id);
        Task<bool> SaveProductAsync(Product product);
        Task<bool> DeleteProductAsync(int id);
    }

    public class ProductService : ServiceBase, IProductService
    {
        private const string _path = "Product";
        private readonly IRestProvider _restProvider;

        public ProductService(IRestProvider restProvider)
        {
            _restProvider = restProvider;
        }

        public async Task<IEnumerable<ProductDTO>> GetProductsAsync()
        {
            var response = await _restProvider.GetAsync(SetPathUrl(_path), id: null);
            var products = await JsonProvider.DeserializeAsync<IEnumerable<ProductDTO>>(response);
            return products;
        }

        public async Task<ProductDTO?> GetProductByIdAsync(int id)
        {
            var response = await _restProvider.GetAsync(SetPathUrl(_path), id.ToString());
            return await JsonProvider.DeserializeAsync<ProductDTO>(response);
        }

        public async Task<bool> SaveProductAsync(Product product)
        {
            var json = JsonSerializer.Serialize(new[] { product });

            var response = await _restProvider.PostAsync(
                SetPathUrl(_path),
                json);

            return JsonSerializer.Deserialize<bool>(response);
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var response = await _restProvider.DeleteAsync(
                SetPathUrl(_path),
                id.ToString());

            return JsonSerializer.Deserialize<bool>(response);
        }
    }
}