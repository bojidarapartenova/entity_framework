using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ProductShop.DTOs.Import
{
    [XmlType("Product")]
    public class ImportProductsDto
    {
        [Required]
        [XmlElement("name")]
        public string Name { get; set; } = null!;

        [Required]
        [XmlElement("price")]
        public decimal Price { get; set; }

        [Required]
        [XmlElement("sellerId")]
        public int SellerId { get; set; } 

        [XmlElement("buyerId")]
        public int? BuyerId { get; set; }
    }
}
