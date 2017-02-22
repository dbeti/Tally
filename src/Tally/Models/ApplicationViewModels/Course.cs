using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Tally.Models
{
    public class Course
    {
        [Required]
        public int CourseId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        public virtual ApplicationUser Professor { get; set; }

        public virtual ICollection<CourseUser> CourseUsers { get; set; }

        public virtual ICollection<Lecture> Lectures { get; set; }
    }
}
