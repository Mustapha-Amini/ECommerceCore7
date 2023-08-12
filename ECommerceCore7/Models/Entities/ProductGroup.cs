using System.ComponentModel.DataAnnotations;

namespace ECommerceCore7.Models.Entities
{
    public class ProductGroup
    {
        [Key]
        public int ProductGroupID { get; set; }

        [Display(Name = "گروه کالا")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [StringLength(100, ErrorMessage = "طول {0} میبایست بین {2} و {1} کاراکتر باشد", MinimumLength = 3)]
        public string ProductGroupTitle { get; set; }

        [Display(Name = "تصویر گروه کالا")]
        //[Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string? ProductGroupImageFilename { get; set; }

        // Navigation Properties
        public virtual List<Product> Products { get; set; } // = new List<Product>();
    }
}
