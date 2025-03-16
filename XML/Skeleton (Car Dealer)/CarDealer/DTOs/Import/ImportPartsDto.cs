using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CarDealer.DTOs.Import
{
    [XmlType("Part")]
    public class ImportPartsDto
    {
        [Required]
        [XmlElement("name")]
        public string Name { get; set; } = null!;

        [Required]
        [XmlElement("price")]
        public string Price { get; set; }=null!;

        [Required]
        [XmlElement("quantity")]
        public string Quantity { get; set; } = null!;

        [Required]
        [XmlElement("supplierId")]
        public string SupplierId { get; set; } = null!;
    }
}
