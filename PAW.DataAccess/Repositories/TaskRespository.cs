using PAW.Repositories;
using PawTask = PAW.Models.Task;

namespace PAW.DataAccess.Repositories;

public interface ITaskRepository : IRepositoryBase<PawTask>
{
    Task<bool> UpsertAsync(PawTask entity, bool isUpdating);
    Task<bool> CreateAsync(PawTask entity);
    Task<bool> DeleteAsync(PawTask entity);
    Task<IEnumerable<PawTask>> ReadAsync();
    Task<PawTask> FindAsync(int id);
    Task<bool> UpdateAsync(PawTask entity);
    Task<bool> UpdateManyAsync(IEnumerable<PawTask> entities);
    Task<bool> ExistsAsync(PawTask entity);
}

public class TaskRepository : RepositoryBase<PawTask>, ITaskRepository
{
}
