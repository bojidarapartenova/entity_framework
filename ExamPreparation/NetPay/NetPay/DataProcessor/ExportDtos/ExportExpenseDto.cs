using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace NetPay.DataProcessor.ExportDtos
{
    [XmlType("Expense")]
    public class ExportExpenseDto
    {
        [XmlElement("ExpenseName")]
        public string ExpenseName { get; set; } = null!;

        [XmlElement("Amount")]
        public string Amount { get; set; }=null!;

        [XmlElement("PaymentDate")]
        public string PaymentDate { get; set; } = null!;

        [XmlElement("ServiceName")]
        public string ServiceName { get; set; } = null!;
    }
}
