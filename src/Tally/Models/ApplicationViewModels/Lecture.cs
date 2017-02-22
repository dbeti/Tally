using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Tally.Models
{
    public class Lecture
    {
        [Required]
        public int LectureId { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required, Display(Name = "Start at")]
        public DateTime StartDate { get; set; }

        [Required]
        public virtual Course Course { get; set; }
    }
}
