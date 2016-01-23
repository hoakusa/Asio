using IdentitySample.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Asio.Models
{
    public class EnrollmentViewModel
    {
        public string UserName { get; set; }
        public Enrollment Enrollment { get; set; }
        public IEnumerable<Course> Courses { get; set; }
        public int[] SelectedCourses { get; set; }
    }
}