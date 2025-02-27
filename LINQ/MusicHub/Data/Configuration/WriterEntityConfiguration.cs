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
    public class WriterEntityConfiguration : IEntityTypeConfiguration<Writer>
    {
        public void Configure(EntityTypeBuilder<Writer> e)
        {
            e.HasKey(p => p.Id);

            e.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(20);

            e.Property(p => p.Pseudonym)
                .IsRequired(false);
        }
    }
}
