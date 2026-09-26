using System.Text.Json.Serialization;

namespace PAW.Models.DTO;

    public class CategoryDTO
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }
        [JsonPropertyName("categoryId")]
        public int CategoryId { get; set; }
        [JsonPropertyName("CategoryName")]
        public string CategoryName { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
        [JsonPropertyName("lastModified")]
        public DateTime? LastModified { get; set; }
        [JsonPropertyName("modifiedBy")]
        public string? ModifiedBy { get; set; }
        
        public static CategoryDTO ConvertFrom(Category category)
        {
            return new CategoryDTO
            {
                Id = Guid.NewGuid(),
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName!,
                Description = category.Description!,
                LastModified = category.LastModified,
                ModifiedBy = category.ModifiedBy
            };
        }

        public static Category ConvertTo(CategoryDTO categoryDTO)
        {
            return new Category
            {
                
                CategoryId = categoryDTO.CategoryId,
                CategoryName = categoryDTO.CategoryName,
                Description = categoryDTO.Description,
                LastModified = categoryDTO.LastModified,
                ModifiedBy = categoryDTO.ModifiedBy
            };
    }
}

