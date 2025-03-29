using Microsoft.EntityFrameworkCore;
using NetPay.Utilities;
using Newtonsoft.Json;
using TravelAgency.Data;
using TravelAgency.Data.Models.Enums;
using TravelAgency.DataProcessor.ExportDtos;

namespace TravelAgency.DataProcessor
{
    public class Serializer
    {
        public static string ExportGuidesWithSpanishLanguageWithAllTheirTourPackages(TravelAgencyContext context)
        {
            ExportGuidesDto[] guides = context.Guides
                .Where(g => g.Language == Language.Spanish)
                .Select(g => new ExportGuidesDto()
                {
                    FullName = g.FullName,
                    TourPackages = g.TourPackagesGuides
                    .Select(tp => tp.TourPackage)
                    .Select(tp => new ExportTourPackagesDto()
                    {
                        Name = tp.PackageName,
                        Description = tp.Description,
                        Price = tp.Price
                    })
                    .OrderByDescending(tp=>tp.Price)
                    .ThenBy(tp=>tp.Name)
                    .ToArray()
                })
                .OrderByDescending(g => g.TourPackages.Count())
                .ThenBy(g => g.FullName)
                .ToArray();

            return XmlHelper.Serialize(guides, "Guides");
        }

        public static string ExportCustomersThatHaveBookedHorseRidingTourPackage(TravelAgencyContext context)
        {
            var customers = context.Customers
                .Where(c=>c.Bookings.Any(b=>b.TourPackage.PackageName== "Horse Riding Tour"))
                .Select(c => new 
                {
                    FullName = c.FullName,
                    PhoneNumber = c.PhoneNumber,
                    Bookings = c.Bookings
                    .Where(b=>b.TourPackage.PackageName== "Horse Riding Tour")
                    .OrderBy(b=>b.BookingDate)
                    .Select(b => new
                    {
                        TourPackageName = b.TourPackage.PackageName,
                        Date = b.BookingDate.ToString("yyyy-MM-dd")
                    })
                    .ToArray()
                })
                .OrderByDescending(c => c.Bookings.Count())
                .ThenBy(c => c.FullName)
                .ToArray();

            return JsonConvert.SerializeObject(customers, Formatting.Indented);
        }
    }
}
