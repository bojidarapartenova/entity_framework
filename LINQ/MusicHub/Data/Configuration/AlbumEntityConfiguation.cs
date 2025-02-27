using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicHub.Data.Models;

namespace MusicHub.Data.Configuration
{
    public class AlbumEntityConfiguation : IEntityTypeConfiguration<Album>
    {
        public void Configure(EntityTypeBuilder<Album> e)
        {
            e.HasKey(a => a.Id);

            e.Property(a => a.Name)
            .IsRequired()
            .HasMaxLength(40);

            e.Property(a => a.ReleaseDate)
            .IsRequired();

            e.Property(a => a.ProducerId)
            .IsRequired(false);

            e.HasOne(a => a.Producer)
                .WithMany(p => p.Albums)
                .HasForeignKey(a => a.ProducerId);
        }
    }
}
