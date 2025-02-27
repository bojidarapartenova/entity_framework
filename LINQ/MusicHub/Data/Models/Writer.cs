using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicHub.Data.Models
{
    public class Writer
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Pseudonym { get; set; }

        public virtual ICollection<Song> Songs { get; set; } = new HashSet<Song>();
    }
}
