using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CarDealer.DTOs.Import
{
    [XmlType("partId")]
    public class ImportCarPartDto
    {
        [Required]
        [XmlAttribute("id")]
        public string Id { get; set; } = null!;
    }
}
