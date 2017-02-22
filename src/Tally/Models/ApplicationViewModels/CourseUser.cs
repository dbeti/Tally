using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Tally.Models
{
    public class CourseUser
    {
        public string Id { get; set; }
        public virtual ApplicationUser User { get; set; }

        public int CourseId { get; set; }
        public virtual Course Course { get; set; }
    }
}
