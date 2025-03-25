using Microsoft.EntityFrameworkCore;
using NetPay.Data;
using NetPay.Data.Models.Enums;
using NetPay.DataProcessor.ExportDtos;
using NetPay.Utilities;
using Newtonsoft.Json;

namespace NetPay.DataProcessor
{
    public class Serializer
    {
        public static string ExportHouseholdsWhichHaveExpensesToPay(NetPayContext context)
        {
            ExportHouseholdsDto[] households = context.Households
                .Include(h=>h.Expenses)
                .ThenInclude(e=>e.Service)
                .OrderBy(h => h.ContactPerson)
                .ToArray()
                .Where(h => h.Expenses.Any(e => e.PaymentStatus != PaymentStatus.Paid))
                .Select(h=>new ExportHouseholdsDto()
                {
                    ContactPerson = h.ContactPerson,
                    Email=h.Email,
                    PhoneNumber=h.PhoneNumber,
                    Expenses=h.Expenses
                    .Where(e=>e.PaymentStatus!= PaymentStatus.Paid)
                    .Select(e => new ExportExpenseDto()
                    {
                        ExpenseName = e.ExpenseName,
                        Amount = e.Amount.ToString("f2"),
                        PaymentDate=e.DueDate.ToString("yyyy-MM-dd"),
                        ServiceName=e.Service.ServiceName
                    })
                    .OrderBy(e=>e.PaymentDate)
                    .ThenBy(e=>e.Amount)
                    .ToArray()
                })
                .ToArray();

            string result = XmlHelper.Serialize(households, "Households");
            return result;
        }

        public static string ExportAllServicesWithSuppliers(NetPayContext context)
        {
            var services = context.Services
                .Select(s => new
                {
                    s.ServiceName,
                    Suppliers = s.SuppliersServices
                    .Select(ss => new
                    {
                        ss.Supplier.SupplierName
                    })
                    .OrderBy(ss => ss.SupplierName)
                    .ToArray()
                })
                .OrderBy(s => s.ServiceName)
                .ToArray();

            string result=JsonConvert
                .SerializeObject(services, Formatting.Indented);

            return result;
        }
    }
}
