using System.Text.Json.Serialization;

namespace PAW.Models.DTO;

public class UserActionDTO
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }
    [JsonPropertyName("userActionId")]
    public int UserActionId { get; set; }
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    public static UserActionDTO ConvertFrom(UserAction userAction)
    {
        return new UserActionDTO
        {
            Id = Guid.NewGuid(),
            UserActionId = (int)(userAction.Id ?? 0),
            Name = userAction.Name,
            Description = userAction.Description
        };
    }

    public static UserAction ConvertTo(UserActionDTO dto)
    {
        return new UserAction
        {
            Id = dto.UserActionId,
            Name = dto.Name,
            Description = dto.Description
        };
    }
}