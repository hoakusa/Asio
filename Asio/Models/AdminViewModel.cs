using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace IdentitySample.Models
{
    public class RoleViewModel
    {
        public string Id { get; set; }
        [Required(AllowEmptyStrings = false)]
        [Display(Name = "RoleName")]
        public string Name { get; set; }
    }

    public class EditUserViewModel
    {
        public string Id { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Name field must be at most 50 characters long.")]
        [Display(Name = "Frist Name")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Name field must be at most 50 characters long.")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        public IEnumerable<SelectListItem> RolesList { get; set; }
    }
}