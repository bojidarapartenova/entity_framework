using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ProductShop.DTOs.Import
{
    public class ImportCategoryProductDTO
    {
        [Required]
        [JsonProperty(nameof(CategoryId))]
        public string CategoryId { get; set; } = null!;

        [Required]
        [JsonProperty(nameof(ProductId))]
        public string ProductId { get; set; } = null!;
    }
}
