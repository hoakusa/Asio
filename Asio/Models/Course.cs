using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Asio.Models
{
    public class Course
    {
        public int Id { get; set; }

        [StringLength(8)]
        [Display(Name = "Course")]
        public string Code { get; set; }

        [StringLength(50)]
        [Display(Name = "Course name")]
        public string Name { get; set; }

        [Range(1, 30)]
        [Display(Name = "Credits")]
        public int Credits { get; set; }

        [Display(Name = "Teacher")]
        public int TeacherId { get; set; }

        public int SemesterId { get; set; }

        public DateTime _CreatedTime = DateTime.Now;
        public DateTime CreatedTime { get { return _CreatedTime; } set { _CreatedTime = value; } }


        public virtual Teacher Teacher { get; set; }
        public virtual Semester Semester { get; set; }
        public ICollection<Enrollment> Enrollments { get; set; }
    }
}