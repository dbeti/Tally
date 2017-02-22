using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Tally.Models
{
    public class Signature
    {
        [Required]
        public int SignatureId { get; set; }

        public String Location { get; set; }

        [Required]
        public virtual Lecture Lecture { get; set; }

        [Required]
        public virtual ApplicationUser Student { get; set; }
    }
}
