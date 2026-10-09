
using Microsoft.EntityFrameworkCore;
using PAW.Models;
using PAW.Repositories;
using System.Linq;

namespace PAW.DataAccess.Repositories;

public interface IUserRoleRepository : IRepositoryBase<UserRole>
{
    Task<bool> UpsertAsync(UserRole entity, bool isUpdating);
    Task<bool> CreateUserRoleAsync(UserRole entity);
    Task<bool> DeleteUserRoleAsync(UserRole entity);
    Task<IEnumerable<UserRole>> ReadAsync();
    Task<UserRole> FindAsync(int id);
    Task<bool> UpdateUserRoleAsync(UserRole entity);
    Task<bool> UpdateManyAsync(IEnumerable<UserRole> entities);
    Task<bool> ExistsAsync(UserRole entity);
}

public class UserRoleRepository
    : RepositoryBase<UserRole>, IUserRoleRepository
{
    public override async Task<UserRole> FindAsync(int id)
    {
        try
        {
            var userRoles = await DbContext.UserRoles
                .AsNoTracking()
                .ToListAsync();

            if (id == 0)
            {
                return userRoles.FirstOrDefault(x => x.Id == null);
            }

            return userRoles.FirstOrDefault(x => x.Id == id);
        }
        catch (Exception ex)
        {
            throw new PAWException(ex);
        }
    }

    public async Task<bool> CreateUserRoleAsync(UserRole entity)
    {
        try
        {
            var existing = await DbContext.UserRoles
                .AsNoTracking()
                .ToListAsync();

            if (!entity.Id.HasValue || entity.Id <= 0)
            {
                entity.Id = existing
                    .Where(x => x.Id.HasValue)
                    .Select(x => x.Id!.Value)
                    .DefaultIfEmpty(0)
                    .Max() + 1;
            }

            var result = await DbContext.Database.ExecuteSqlRawAsync(
                """
                INSERT INTO dbo.UserRoles (Id, RoldID, UserID)
                VALUES ({0}, {1}, {2})
                """,
                entity.Id,
                entity.RoldId,
                entity.UserId);

            return result > 0;
        }
        catch (Exception ex)
        {
            throw new PAWException(ex);
        }
    }

    public async Task<bool> UpdateUserRoleAsync(UserRole entity)
    {
        try
        {
            if (!entity.Id.HasValue)
            {
                return false;
            }

            var result = await DbContext.Database.ExecuteSqlRawAsync(
                """
                UPDATE dbo.UserRoles
                SET RoldID = {0},
                    UserID = {1}
                WHERE Id = {2}
                """,
                entity.RoldId,
                entity.UserId,
                entity.Id);

            return result > 0;
        }
        catch (Exception ex)
        {
            throw new PAWException(ex);
        }
    }

    public async Task<bool> DeleteUserRoleAsync(UserRole entity)
    {
        try
        {
            if (!entity.Id.HasValue)
            {
                return false;
            }

            var result = await DbContext.Database.ExecuteSqlRawAsync(
                """
                DELETE FROM dbo.UserRoles
                WHERE Id = {0}
                """,
                entity.Id);

            return result > 0;
        }
        catch (Exception ex)
        {
            throw new PAWException(ex);
        }
    }
}
