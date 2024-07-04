using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModels
{
    public class Publicacion
    {
        public int PublicacionId { get; set; }
        public int AutorId { get; set; }

        public string Title { get; set; }
        public string Content { get; set; }
        public DateOnly CreatedAt { get; set; }

        public string PostStatus { get; set; }
        public Autor? Autor { get; set; }
    }
}
