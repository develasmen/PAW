using PAW.Architecture;
using PAW.Architecture;
using PAW.Architecture.Providers;
using PAW.data.models;

namespace PAW.Web.Services;

public class ProductService
{
    private const string BaseUrl = "https://localhost:7269/api/Products/";

    private readonly IRestProvider _restProvider;

    public ProductService(IRestProvider restProvider)
    {
        _restProvider = restProvider;
    }

    public async Task<List<Product>> GetAllAsync()
    {
        var json = await _restProvider.GetAsync(BaseUrl, null);
        return JsonProvider.DeserializeSimple<List<Product>>(json) ?? new List<Product>();
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        var json = await _restProvider.GetAsync(BaseUrl, id.ToString());
        return JsonProvider.DeserializeSimple<Product>(json);
    }
}