using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CarDealer.DTOs.Import
{
    [XmlType("Customer")]
    public class ImportCustomerDto
    {
        [Required]
        [XmlElement("name")]
        public string Name { get; set; } = null!;

        [Required]
        [XmlElement("birthDate")]
        public string BirthDate { get; set; } = null!;

        [Required]
        [XmlElement("isYoungDriver")]
        public string IsYoungDriver { get; set; } = null!;
    }
}
