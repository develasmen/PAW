using PAW.Models;
using PAW.Repositories;

namespace PAW.DataAccess.Repositories;

public interface IUserRoleRepository : IRepositoryBase<UserRole>
{
    Task<bool> UpsertAsync(UserRole entity, bool isUpdating);
    Task<bool> CreateAsync(UserRole entity);
    Task<bool> DeleteAsync(UserRole entity);
    Task<IEnumerable<UserRole>> ReadAsync();
    Task<UserRole> FindAsync(int id);
    Task<bool> UpdateAsync(UserRole entity);
    Task<bool> UpdateManyAsync(IEnumerable<UserRole> entities);
    Task<bool> ExistsAsync(UserRole entity);
}

public class UserRoleRepository : RepositoryBase<UserRole>, IUserRoleRepository
{
}