using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicHub.Data.Models;

namespace MusicHub.Data.Configuration
{
    public class SongEntityConfiguration : IEntityTypeConfiguration<Song>
    {
        public void Configure(EntityTypeBuilder<Song> e)
        {
            e.HasKey(s => s.Id);

            e.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(20);

            e.Property(s => s.Duration)
            .IsRequired();

            e.Property(s => s.CreatedOn)
            .IsRequired();

            e.Property(s => s.Genre)
            .IsRequired();

            e.Property(s => s.Price)
            .IsRequired();

            e.Property(s => s.AlbumId)
            .IsRequired(false);

            e.Property(s => s.WriterId)
            .IsRequired();

            e.HasOne(s => s.Album)
                .WithMany(a => a.Songs)
                .HasForeignKey(s => s.AlbumId);

            e.HasOne(s => s.Writer)
                .WithMany(w => w.Songs)
                .HasForeignKey(s => s.WriterId);
        }
    }
}
