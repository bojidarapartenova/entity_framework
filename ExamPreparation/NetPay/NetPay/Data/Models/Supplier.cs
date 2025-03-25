using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace NetPay.Data.Models
{
    public class Supplier
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(60)]
        public string SupplierName { get; set; } = null!;

        public virtual ICollection<SupplierService> SuppliersServices {  get; set; }=new HashSet<SupplierService>();
    }
}
