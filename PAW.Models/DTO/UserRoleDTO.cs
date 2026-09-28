using System.Text.Json.Serialization;

namespace PAW.Models.DTO;

public class UserRoleDTO
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }
    [JsonPropertyName("userRoleId")]
    public int UserRoleId { get; set; }
    [JsonPropertyName("roleId")]
    public int RoleId { get; set; }
    [JsonPropertyName("userId")]
    public int UserId { get; set; }

    public static UserRoleDTO ConvertFrom(UserRole userRole)
    {
        return new UserRoleDTO
        {
            Id = Guid.NewGuid(),
            UserRoleId = (int)(userRole.Id ?? 0),
            RoleId = (int)(userRole.RoldId ?? 0),
            UserId = (int)(userRole.UserId ?? 0)
        };
    }
}