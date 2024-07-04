using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModels.Dto
{
    public class PublicacionDto
    {
        [Required]
        public int PublicacionId { get; set; }
        [Required]
        public int AutorId { get; set; }
        [StringLength(50)]
        [Required]
        public string Title { get; set; }
        [Required]
        public string Content { get; set; }
        public DateOnly CreatedAt { get; set; }
        [Required]
        public string PostStatus { get; set; }
    }
}
