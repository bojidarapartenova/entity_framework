using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetPay.Data.Models;
using Newtonsoft.Json;

namespace NetPay.DataProcessor.ImportDtos
{
    public class ImportExpenseDto
    {
        [Required]
        [MinLength(5)]
        [MaxLength(50)]
        [JsonProperty(nameof(ExpenseName))]
        public string ExpenseName { get; set; } = null!;

        [Required]
        [Range(typeof(decimal), "0.01", "100000")]
        [JsonProperty(nameof(Amount))]
        public decimal Amount {  get; set; }

        [Required]
        [JsonProperty(nameof(DueDate))]
        public string DueDate { get; set; }=null!;

        [Required]
        [JsonProperty(nameof(PaymentStatus))]
        public string PaymentStatus { get; set; } = null!;

        [Required]
        [JsonProperty(nameof(HouseholdId))]
        public int HouseholdId {  get; set; }

        [Required]
        [JsonProperty(nameof(ServiceId))]
        public int ServiceId {  get; set; }
    }
}
