using System.Text.Json.Serialization;

namespace PAW.Models.DTO;

public class ComponentDTO
{
    [JsonPropertyName("id")]
    public decimal Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;

    [JsonPropertyName("content")]
    public string Content { get; set; } = null!;

    public static ComponentDTO ConvertFrom(Component component)
    {
        return new ComponentDTO
        {
            Id = component.Id,
            Name = component.Name,
            Content = component.Content
        };
    }

    public static Component ConvertTo(ComponentDTO componentDTO)
    {
        return new Component
        {
            Id = componentDTO.Id,
            Name = componentDTO.Name,
            Content = componentDTO.Content
        };
    }
}