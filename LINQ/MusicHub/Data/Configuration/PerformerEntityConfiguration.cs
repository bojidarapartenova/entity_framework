using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicHub.Data.Models;

namespace MusicHub.Data.Configuration
{
    public class PerformerEntityConfiguration : IEntityTypeConfiguration<Performer>
    {
        public void Configure(EntityTypeBuilder<Performer> e)
        {
            e.HasKey(p => p.Id);

            e.Property(p => p.FirstName)
                .IsRequired()
                .HasMaxLength(20);

            e.Property(p => p.LastName)
                .IsRequired()
                .HasMaxLength(20);

            e.Property(p => p.Age)
                .IsRequired();

            e.Property(p => p.NetWorth)
                .IsRequired();
        }
    }
}
