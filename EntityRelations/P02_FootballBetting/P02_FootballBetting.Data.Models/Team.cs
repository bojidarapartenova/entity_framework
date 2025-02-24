﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P02_FootballBetting.Data.Models
{
    public class Team
    {
        [Key]
        public int TeamId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        [MaxLength(2048)]
        public string? LogoUrl { get; set; }

        [Required]
        [MaxLength(5)]
        public string Initials { get; set; } = null!;

        public decimal Budget { get; set; }

        [ForeignKey(nameof(PrimaryKitColor))]
        public int PrimaryKitColorId { get; set; }
        public virtual Color PrimaryKitColor { get; set; } = null!;


        [ForeignKey(nameof(SecondaryKitColor))]
        public int SecondaryKitColorId { get; set;}
        public virtual Color SecondaryKitColor { get;set; } = null!;

        [ForeignKey(nameof(Town))]
        public int TownId { get; set; }
        public virtual Town Town { get; set; } = null!;


        [InverseProperty(nameof(Game.HomeTeam))]
        public virtual ICollection<Game> HomeGames { get; set; } = new HashSet<Game>();

        [InverseProperty(nameof(Game.AwayTeam))]
        public virtual ICollection<Game> AwayGames { get; set; }=new HashSet<Game>();

        public virtual ICollection<Player> Players { get; set; } = new HashSet<Player>();
    }
}
