using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ProductShop.DTOs.Import
{
    using System.ComponentModel.DataAnnotations;
    using Microsoft.EntityFrameworkCore.Query.Internal;
    using Newtonsoft.Json;
    public class ImportUserDTO
    {
        [JsonProperty("firstName")]
        public string? FirstName { get; set; }

        [Required]
        [JsonProperty("lastName")]
        public string LastName { get; set; } = null!;

        [JsonProperty("age")]
        public int? Age { get; set; }
    }
}
