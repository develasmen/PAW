using PawTask = PAW.Models.Task;

namespace PAW.MinimalAPI.Repositories;

public interface ITasksRepository
{
    Task<IReadOnlyList<PawTask>> GetAllAsync(CancellationToken cancellationToken);
    Task<PawTask?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<PawTask> CreateAsync(PawTask task, CancellationToken cancellationToken);
    Task<PawTask?> UpdateAsync(int id, PawTask task, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
}