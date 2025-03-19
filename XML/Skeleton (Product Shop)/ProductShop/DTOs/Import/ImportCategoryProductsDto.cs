using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ProductShop.DTOs.Import
{
    [XmlType("CategoryProduct")]
    public class ImportCategoryProductsDto
    {
        [Required]
        [XmlElement("CategoryId")]
        public int CategoryId { get; set; } 

        [Required]
        [XmlElement("ProductId")]
        public int ProductId { get; set; } 
    }
}
