using System.ComponentModel.DataAnnotations;

namespace TravelAgency.Data.Models
{
    public class TourPackage
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MinLength(2)]
        [MaxLength(40)]
        public string PackageName { get; set; } = null!;

        [MaxLength(200)]
        public string? Description {  get; set; }

        [Required]
        public decimal Price {  get; set; }

        public virtual ICollection<Booking> Bookings { get; set; } = new HashSet<Booking>();
        public virtual ICollection<TourPackageGuide> TourPackagesGuides { get; set; }= new HashSet<TourPackageGuide>();
    }
}