using System.Collections.Generic;

namespace Asio.Models
{
    public class CourseViewModel
    {
        public Course Course { get; set; }
        public IEnumerable<Enrollment> Enrollments { get; set; }
    }
}