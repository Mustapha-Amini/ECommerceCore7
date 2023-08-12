using ECommerceCore7.Models.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceCore7.Models.Entities
{
    //[Bind("ProductID,ProductGroupID,ProductTitle,ProductShortDescription,ProductLongDescription,ProductPrice")]
    public class Page
    {
        [Key]
        public int PageID { get; set; }

        [Display(Name = "گروه صفحه")]
        public int PageGroupID { get; set; }

        [Display(Name = "نویسنده")]
        public int UserID { get; set; }

        [Display(Name = "عنوان صفحه")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [StringLength(100, ErrorMessage = "طول {0} میبایست بین {2} و {1} کاراکتر باشد", MinimumLength = 3)]
        public string PageTitle { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "محتوای کوتاه صفحه")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string PageShortContent { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "محتوای کامل صفحه")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string PageLongContent { get; set; }


        [Display(Name = "تصویر صفحه")]
        public string? PageImageFilename { get; set; }

        [Display(Name = "تاریخ آخرین تغییر")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime PageLastModified { get; set; }

        [Display(Name = "مسیر صفحه")]
        public string? PageRoute { get; set; }

        [Display(Name = "نمایش صفحه به کاربران")]

        //[UIHint("MyBoolean")]
        public bool PageEnabled { get; set; }

        [Display(Name = "نمایش مستقیم لینک صفحه در منوی اصلی سایت")]
        public bool PageShowLinkDirectlyInTopMenu { get; set; } = false;

        [Display(Name = "نمایش لینک صفحه در منوی جداگانه گروه صفحه")]
        public bool PageShowLinkInTopMenuInGroup { get; set; } = false;

        [Display(Name = "نمایش لینک صفحه در فوتر سایت")]
        public bool PageShowLinkInFooter { get; set; } = false;

        [Display(Name = "تگ های متا صفحه")]
        public string? PageMetaTags { get; set; }

        [Display(Name = "شرح متا صفحه")]
        public string? PageMetaDescription { get; set; }


        // Navigation Properties
        [ForeignKey("PageGroupID")]
        [Display(Name = "گروه صفحه")]
        public virtual PageGroup PageGroup { get; set; }


        [Display(Name = "نویسنده")]
        [ForeignKey("UserID")]
        public virtual ApplicationUser? User { get; set; }

        public virtual List<Video> Videos { get; set; }

        public virtual List<PageTag> PageTags { get; set; } = new List<PageTag>();

    }
}
