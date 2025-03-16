using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CarDealer.DTOs.Import
{
    [XmlType("Sale")]
    public class ImportSaleDto
    {
        [Required]
        [XmlElement("carId")]
        public string CarId { get; set; } = null!;

        [Required]
        [XmlElement("customerId")]
        public string CustomerId { get; set; } = null!;

        [Required]
        [XmlElement("discount")]
        public string Discount { get; set; } = null!;
    }
}
