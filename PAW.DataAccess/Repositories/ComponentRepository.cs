using Microsoft.EntityFrameworkCore;
using PAW.Models;
using PAW.Repositories;

namespace PAW.DataAccess.Repositories;

public interface IComponentRepository : IRepositoryBase<Component>
{
    Task<Component?> FindByIdAsync(decimal id);
}

public class ComponentRepository
    : RepositoryBase<Component>, IComponentRepository
{
    public async Task<Component?> FindByIdAsync(decimal id)
    {
        return await DbContext.Components
            .FirstOrDefaultAsync(x => x.Id == id);
    }
}