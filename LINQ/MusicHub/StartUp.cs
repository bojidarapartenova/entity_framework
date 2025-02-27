namespace MusicHub
{
    using System;
    using System.Text;
    using Data;
    using Initializer;

    public class StartUp
    {
        public static void Main()
        {
            MusicHubDbContext context =
                new MusicHubDbContext();

            DbInitializer.ResetDatabase(context);

            const int producerId = 9;
            string result = ExportAlbumsInfo(context, producerId);
            Console.WriteLine(result);

            Console.WriteLine("-------------------------------------------");

            const int duration = 4;
            string secondResult= ExportSongsAboveDuration(context, duration);
            Console.WriteLine(secondResult);
        }

        //All Albums Produced by Given Producer

        public static string ExportAlbumsInfo(MusicHubDbContext context, int producerId)
        {
            StringBuilder sb = new StringBuilder();

            var albums = context
                .Albums
                .Where(a => a.ProducerId.HasValue && a.ProducerId.Value == producerId)
                .Select(a => new
                {
                    AlbumName = a.Name,
                    ReleaseDate = a.ReleaseDate.ToString("MM/dd/yyyy"),
                    ProducerName = a.Producer!.Name,
                    Songs = a.Songs
                    .Select(s => new
                    {
                        SongName = s.Name,
                        Price = s.Price.ToString("F2"),
                        SongWriterName = s.Writer.Name
                    })
                    .OrderByDescending(s => s.SongName)
                    .ThenBy(s => s.SongWriterName)
                    .ToArray(),
                    AlbumPrice = a.Price
                })
                .ToArray();

            albums = albums.OrderByDescending(a => a.AlbumPrice).ToArray();

            foreach (var album in albums)
            {
                sb.AppendLine($"-AlbumName: {album.AlbumName}");
                sb.AppendLine($"-ReleaseDate: {album.ReleaseDate}");
                sb.AppendLine($"-ProducerName: {album.ProducerName}");
                sb.AppendLine("-Songs:");

                int index = 1;

                foreach (var song in album.Songs)
                {
                    sb.AppendLine($"---#{index++}");
                    sb.AppendLine($"---SongName: {song.SongName}");
                    sb.AppendLine($"---Price: {song.Price}");
                    sb.AppendLine($"---Writer: {song.SongWriterName}");
                }

                sb.AppendLine($"-AlbumPrice: {album.AlbumPrice.ToString("F2")}");
            }

            return sb.ToString().TrimEnd();
        }

        //Songs Above Given Duration
        public static string ExportSongsAboveDuration(MusicHubDbContext context, int duration)
        {
            StringBuilder sb = new StringBuilder();

            TimeSpan durationSpan = TimeSpan.FromSeconds(duration);

            var songs = context
                .Songs
                .Where(s => s.Duration > durationSpan)
                .Select(s => new
                {
                    SongName = s.Name,
                    SongPerformers = s.SongPerformers
                    .Select(sp => new
                    {
                        PerformerFirstName = sp.Performer.FirstName,
                        PerformerLastName = sp.Performer.LastName
                    })
                    .OrderBy(sp => sp.PerformerFirstName)
                    .ThenBy(sp => sp.PerformerLastName)
                    .ToArray(),
                    WriterName = s.Writer.Name,
                    AlbumProducer = (s.Album != null) ?
                    (s.Album.Producer != null ? s.Album.Producer.Name : null)
                    : null,
                    Duration = s.Duration.ToString("c")
                })
                .OrderBy(s => s.SongName)
                .ThenBy(s => s.WriterName)
                .ToArray();

            int index = 1;
            foreach (var song in songs)
            {
                sb.AppendLine($"-Song #{index++}");
                sb.AppendLine($"---SongName: {song.SongName}");
                sb.AppendLine($"---Writer: {song.WriterName}");

                foreach(var performer in song.SongPerformers)
                {
                    sb.AppendLine($"---Performer: {performer.PerformerFirstName} {performer.PerformerLastName}");
                }

                sb.AppendLine($"---AlbumProducer: {song.AlbumProducer}");
                sb.AppendLine($"---Duration: {song.Duration}");
            }

            return sb.ToString().TrimEnd();
        }
    }
}
