using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TravelAgency.DataProcessor.ImportDtos
{
    public class ImportBookings
    {
        [Required]
        [JsonProperty(nameof(BookingDate))]
        public string BookingDate { get; set; } = null!;

        [Required]
        [MinLength(4)]
        [MaxLength(60)]
        [JsonProperty(nameof(CustomerName))]
        public string CustomerName { get; set; } = null!;

        [Required]
        [MinLength(2)]
        [MaxLength(40)]
        [JsonProperty(nameof(TourPackageName))]
        public string TourPackageName { get; set; } = null!;
    }
}
