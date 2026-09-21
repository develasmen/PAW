using PAW.data.models;

namespace PAW.Repositories;

public interface IProductRepository : IRepositoryBase<Product>
{
}

public class ProductRepository : RepositoryBase<Product>, IProductRepository
{
}