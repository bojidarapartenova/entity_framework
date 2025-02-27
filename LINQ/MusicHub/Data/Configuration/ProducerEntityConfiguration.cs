using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicHub.Data.Models;

namespace MusicHub.Data.Configuration
{
    public class ProducerEntityConfiguration : IEntityTypeConfiguration<Producer>
    {
        public void Configure(EntityTypeBuilder<Producer> e)
        {
            e.HasKey(p=>p.Id);

            e.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(30);

            e.Property(p => p.Pseudonym)
                .IsRequired(false);

            e.Property(p=>p.PhoneNumber)
                .IsRequired(false);
        }
    }
}
