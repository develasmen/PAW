using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using PawTask = PAW.Models.Task;

namespace PAW.Models.DTO;

public class PawTaskDTO
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(255)]
    public string? Name { get; set; }

    [JsonPropertyName("description")]
    [StringLength(1000)]
    public string? Description { get; set; }

    [JsonPropertyName("status")]
    [StringLength(50)]
    public string? Status { get; set; }

    [JsonPropertyName("dueDate")]
    public DateTime? DueDate { get; set; }

    [JsonPropertyName("createdAt")]
    public DateTime? CreatedAt { get; set; }

    [JsonPropertyName("lastModified")]
    public DateTime? LastModified { get; set; }

    [JsonPropertyName("modifiedBy")]
    [StringLength(255)]
    public string? ModifiedBy { get; set; }

    public static PawTaskDTO ConvertFrom(PawTask task)
    {
        return new PawTaskDTO
        {
            Id = task.Id,
            Name = task.Name,
            Description = task.Description,
            Status = task.Status,
            DueDate = task.DueDate,
            CreatedAt = task.CreatedAt,
            LastModified = task.LastModified,
            ModifiedBy = task.ModifiedBy
        };
    }

    public static PawTask ConvertTo(PawTaskDTO taskDTO)
    {
        return new PawTask
        {
            Id = taskDTO.Id,
            Name = taskDTO.Name,
            Description = taskDTO.Description,
            Status = taskDTO.Status,
            DueDate = taskDTO.DueDate,
            CreatedAt = taskDTO.CreatedAt,
            LastModified = taskDTO.LastModified,
            ModifiedBy = taskDTO.ModifiedBy
        };
    }
}