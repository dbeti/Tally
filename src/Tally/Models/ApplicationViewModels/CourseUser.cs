using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tally.Models.ApplicationViewModels
{
    public class CourseUser
    {
        public string Id { get; set; }
        public ApplicationUser User { get; set; }

        public int CourseId { get; set; }
        public Course Course { get; set; }
    }
}
