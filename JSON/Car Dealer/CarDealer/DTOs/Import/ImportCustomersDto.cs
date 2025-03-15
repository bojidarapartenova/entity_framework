using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CarDealer.DTOs.Import
{
    public class ImportCustomersDto
    {
        [Required]
        [JsonProperty("name")]
        public string Name { get; set; } = null!;

        [Required]
        [JsonProperty("birthDate")]
        public string BirthDate { get; set; } = null!;

        [Required]
        [JsonProperty("isYoungDriver")]
        public string IsYoungDriver { get; set; }=null!;
    }
}
