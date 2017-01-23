﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Tally.Models.ApplicationViewModels
{
    public class Course
    {
        [Required]
        public int CourseId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        public ICollection<ApplicationUser> Users { get; set; }

        public ICollection<Lecture> Lectures { get; set; }
    }
}
