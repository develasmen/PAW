using Microsoft.AspNetCore.Mvc;
using PAW.DataAccess.Repositories;
using PAW.Models.DTO;
using PawTask = PAW.Models.Task;

namespace PAW.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TaskController(
        ILogger<TaskController> logger,
        ITaskRepository taskRepository) : ControllerBase
    {
        [HttpGet(Name = "GetTasks")]
        public async Task<IEnumerable<PawTaskDTO>> GetAll()
        {
            var tasks = await taskRepository.ReadAsync() ?? [];

            return tasks.Select(PawTaskDTO.ConvertFrom);
        }

        [HttpGet("{id:int}", Name = "GetTaskById")]
        public async Task<ActionResult<PawTaskDTO>> GetById(int id)
        {
            var task = await taskRepository.FindAsync(id);

            if (task == null)
            {
                return NotFound();
            }

            return PawTaskDTO.ConvertFrom(task);
        }

        [HttpPost]
        public async Task<bool> Save([FromBody] IEnumerable<PawTask> tasks)
        {
            foreach (var task in tasks)
            {
                if (task.Id > 0)
                    await taskRepository.UpdateAsync(task);
                else
                    await taskRepository.CreateAsync(task);
            }

            return true;
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            var task = await taskRepository.FindAsync(id);

            if (task == null)
                return NotFound();

            var result = await taskRepository.DeleteAsync(task);

            return Ok(result);
        }
    }
}
