using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CarDealer.DTOs.Import
{
    public class ImportCarsDto
    {
        [Required]
        [JsonProperty("make")]
        public string Make { get; set; } = null!;

        [Required]
        [JsonProperty("model")]
        public string Model { get; set; } = null!;

        [Required]
        [JsonProperty("traveledDistance")]
        public string TraveledDistance { get; set; } = null!;

        [Required]
        [JsonProperty("partsId")]
        public int[] PartsId { get; set; } = null!;
    }
}
