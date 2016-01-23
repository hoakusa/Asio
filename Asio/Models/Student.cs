using IdentitySample.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Asio.Models
{
    public class Student
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("User")]
        public string UserId { get; set; }

        public int? GroupId { get; set; }

        public virtual ApplicationUser User { get; set; }
        public virtual Group Group { get; set; }
        public virtual ICollection<Enrollment> Enrollments { get; set; }
    }
}