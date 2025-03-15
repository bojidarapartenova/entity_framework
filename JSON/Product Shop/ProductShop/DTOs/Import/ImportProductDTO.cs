using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ProductShop.DTOs.Import
{
    public class ImportProductDTO
    {
        [Required]
        [JsonProperty("Name")]
        public string Name { get; set; } = null!;

        [Required]
        [JsonProperty("Price")]
        public string Price { get; set; } = null!;

        [JsonProperty("BuyerId")]
        public string? BuyerId { get; set; }

        [Required]
        [JsonProperty("SellerId")]
        public string SellerId { get; set; }=null!;
    }
}
