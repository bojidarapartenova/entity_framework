using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text;
using NetPay.Utilities;
using Newtonsoft.Json;
using TravelAgency.Data;
using TravelAgency.Data.Models;
using TravelAgency.DataProcessor.ImportDtos;

namespace TravelAgency.DataProcessor
{
    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data format!";
        private const string DuplicationDataMessage = "Error! Data duplicated.";
        private const string SuccessfullyImportedCustomer = "Successfully imported customer - {0}";
        private const string SuccessfullyImportedBooking = "Successfully imported booking. TourPackage: {0}, Date: {1}";

        public static string ImportCustomers(TravelAgencyContext context, string xmlString)
        {
            StringBuilder sb=new StringBuilder();

            ImportCustomersDto[]? customersDtos = XmlHelper
                .Deserialize<ImportCustomersDto[]>(xmlString, "Customers");

            if (customersDtos != null && customersDtos.Length > 0)
            {
                ICollection<Customer> customers = new List<Customer>();

                foreach (var customerDto in customersDtos)
                {
                    if(!IsValid(customerDto))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    bool isAlreadyImported=context.Customers
                        .Any(c=>c.FullName==customerDto.FullName ||
                        c.Email==customerDto.Email || c.PhoneNumber==customerDto.PhoneNumber);

                    bool isToBeImported=customers
                        .Any(c => c.FullName == customerDto.FullName ||
                        c.Email == customerDto.Email || c.PhoneNumber == customerDto.PhoneNumber);

                    if(isAlreadyImported || isToBeImported)
                    {
                        sb.AppendLine(DuplicationDataMessage);
                        continue;
                    }

                    Customer customer = new Customer()
                    {
                        FullName = customerDto.FullName,
                        Email = customerDto.Email,
                        PhoneNumber = customerDto.PhoneNumber
                    };
                    customers.Add(customer);

                    string successMessage = string.Format(SuccessfullyImportedCustomer, customerDto.FullName);
                    sb.AppendLine(successMessage);
                }
                context.Customers.AddRange(customers);
                context.SaveChanges();
            }
            return sb.ToString().TrimEnd();
        }

        public static string ImportBookings(TravelAgencyContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();

            ImportBookings[]? bookingsDtos=JsonConvert
                .DeserializeObject<ImportBookings[]>(jsonString);

            if (bookingsDtos != null && bookingsDtos.Length > 0)
            {
                ICollection<Booking> bookings = new List<Booking>();

                foreach (var bookingDto in bookingsDtos)
                {
                    if (!IsValid(bookingDto))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    bool isDateValid = DateTime.TryParseExact
                        (bookingDto.BookingDate, "yyyy-MM-dd", CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out DateTime bookingDate);

                    var customer = context.Customers
                        .FirstOrDefault(c => c.FullName == bookingDto.CustomerName);

                    var tourPackage = context.TourPackages
                        .FirstOrDefault(tp => tp.PackageName == bookingDto.TourPackageName);

                    if(!isDateValid)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    if (customer != null && tourPackage != null)
                    {
                        Booking booking = new Booking()
                        {
                            BookingDate = bookingDate,
                            Customer = customer,
                            TourPackage = tourPackage
                        };
                        bookings.Add(booking);

                        string successMessage = string.Format(SuccessfullyImportedBooking,
                            bookingDto.TourPackageName, bookingDto.BookingDate);
                        sb.AppendLine(successMessage);
                    }
                }
                context.Bookings.AddRange(bookings);
                context.SaveChanges();
            }
            return sb.ToString().TrimEnd();
        }

        public static bool IsValid(object dto)
        {
            ValidationContext validationContext = new ValidationContext(dto);
            List<ValidationResult> validationResults = new List<ValidationResult>();

            bool isValid = Validator
                .TryValidateObject(dto, validationContext, validationResults, true);

            return isValid;
        }
    }
}
