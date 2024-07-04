using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModels
{
    public class Autor
    {
        public int AutorId { get; set; }
        public string Name { get; set; }
        public string Biography { get; set; }
        private ICollection<Autor>? Autores { get; set; }
    }
}
