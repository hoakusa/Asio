using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Asio.Models
{
    public class Semester
    {
        public int Id { get; set; }

        [Required]
        [StringLength(10)]
        [Display(Name = "Semester")]
        public string Name { get; set; }

        public virtual ICollection<Course> Courses { get; set; }
    }
}