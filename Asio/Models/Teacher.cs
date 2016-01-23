using IdentitySample.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Asio.Models
{
    public class Teacher
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("User")]
        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; }
        public virtual ICollection<Course> Courses { get; set; }
    }
}