using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ECommerceCore7.Models.Entities
{
    public class PageGroup
    {
        [Key]
        public int PageGroupID { get; set; }

        [Display(Name = "گروه صفحه")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [StringLength(100, ErrorMessage = "طول {0} میبایست بین {2} و {1} کاراکتر باشد", MinimumLength = 3)]
        public string PageGroupTitle { get; set; }

        [Display(Name = "آیکون گروه صفحه")]
        public string? PageGroupIcon { get; set; }


        // Navigation Properties
        public virtual List<Page> Pages { get; set; }
    }
}
