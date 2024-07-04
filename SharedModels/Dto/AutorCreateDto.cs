using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModels.Dto
{
    public class AutorCreateDto
    {
        public string Name { get; set; }
        [StringLength(50)]
        [Required]
        public string Biography { get; set; }
    }
}
