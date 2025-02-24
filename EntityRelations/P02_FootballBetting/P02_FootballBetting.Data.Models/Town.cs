using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P02_FootballBetting.Data.Models
{
    public class Town
    {
        [Key]
        public int TownId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        [ForeignKey(nameof(Country))]
        public int CountryId { get; set; }
        public virtual Country Country { get; set; } = null!;

        public virtual ICollection<Team> Teams { get; set; }=new HashSet<Team>();

        public virtual ICollection<Player> Players { get; set; } = new HashSet<Player>();
    }
}
