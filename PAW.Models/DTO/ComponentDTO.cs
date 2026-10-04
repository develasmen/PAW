using System.Text.Json.Serialization;

namespace PAW.Models.DTO;

public class ComponentDTO
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("componentId")]
    public int ComponentId { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("content")]
    public string? Content { get; set; }

    public static ComponentDTO ConvertFrom(Component component)
    {
        return new ComponentDTO
        {
            Id = Guid.NewGuid(),
            ComponentId = (int)component.Id,
            Name = component.Name,
            Content = component.Content
        };
    }

    public static Component ConvertTo(ComponentDTO dto)
    {
        return new Component
        {
            Id = dto.ComponentId,
            Name = dto.Name!,
            Content = dto.Content!
        };
    }
}