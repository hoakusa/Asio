using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Asio.Models
{
    public class Group
    {
        public int Id { get; set; }

        [Required]
        [StringLength(10)]
        [Display(Name = "Group")]
        public string Name { get; set; }
        public virtual ICollection<Student> Students { get; set; }
    }
}