using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModels.Dto
{
    public class PublicacionUpdateDto
    {
        [Required]
        public int AutorId { get; set; }
        [Required]
        public int PublicacionId { get; set; }
        [StringLength(25)]
        [Required]
        public string Title { get; set; }
        [Required]
        public string Content { get; set; }
        [Required]
        public string PostStatus { get; set; }
    }
}
