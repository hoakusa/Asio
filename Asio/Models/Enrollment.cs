using IdentitySample.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Asio.Models
{
    public class Enrollment
    {
        public int Id { get; set; }

        [Display(Name = "Name")]
        public int StudentId { get; set; }

        [Display(Name = "Course")]
        public int CourseId { get; set; }

        [Range(0, 5)]
        [Display(Name = "Grade")]
        public int? Grade { get; set; }
        //public int Tesst { get; set; }

        [DefaultValue(false)]
        public bool IsPassed { get; set; }

        public DateTime _CreatedTime = DateTime.Now;
        public DateTime CreatedTime { get { return _CreatedTime; } set { _CreatedTime = value; } }

        public virtual Student Student { get; set; }
        public virtual Course Course { get; set; }
    }
}