using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ECommerceCore7.Models.Identity
{
    public class ApplicationRole : IdentityRole<int>
    {
        public virtual ICollection<ApplicationUserRole>? UserRoles { get; set; }
        public virtual ICollection<ApplicationRoleClaim>? RoleClaims { get; set; }

        [Display(Name = "نام گروه")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public override string Name { get; set; }

        [Display(Name = "نام نمایشی گروه")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public override string NormalizedName { get; set; }
    }
}