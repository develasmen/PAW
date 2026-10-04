using System.Text.Json.Serialization;

namespace PAW.Models.DTO;

public class InventoryDTO
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("inventoryId")]
    public int InventoryId { get; set; }

    [JsonPropertyName("unitPrice")]
    public decimal? UnitPrice { get; set; }

    [JsonPropertyName("unitsInStock")]
    public int? UnitsInStock { get; set; }

    [JsonPropertyName("lastUpdated")]
    public DateTime? LastUpdated { get; set; }

    [JsonPropertyName("productId")]
    public int? ProductId { get; set; }

    [JsonPropertyName("dateAdded")]
    public DateTime? DateAdded { get; set; }

    [JsonPropertyName("modifiedBy")]
    public string? ModifiedBy { get; set; }

    [JsonPropertyName("products")]
    public IEnumerable<ProductDTO> Products { get; set; } = [];

    public static InventoryDTO ConvertFrom(Inventory inventory)
    {
        return new InventoryDTO
        {
            Id = Guid.NewGuid(),
            InventoryId = inventory.InventoryId,
            UnitPrice = inventory.UnitPrice,
            UnitsInStock = inventory.UnitsInStock,
            LastUpdated = inventory.LastUpdated,
            ProductId = inventory.ProductId,
            DateAdded = inventory.DateAdded,
            ModifiedBy = inventory.ModifiedBy,

            Products = inventory.Products?
                .Select(ProductDTO.ConvertFrom)
                .ToList() ?? []
        };
    }

    public static Inventory ConvertTo(InventoryDTO dto)
    {
        return new Inventory
        {
            InventoryId = dto.InventoryId,
            UnitPrice = dto.UnitPrice,
            UnitsInStock = dto.UnitsInStock,
            LastUpdated = dto.LastUpdated,
            ProductId = dto.ProductId,
            DateAdded = dto.DateAdded,
            ModifiedBy = dto.ModifiedBy
        };
    }
}