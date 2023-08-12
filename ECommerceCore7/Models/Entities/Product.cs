using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceCore7.Models.Entities
{
    public class Product
    {
        [Key]
        public int ProductID { get; set; }

        [Display(Name = "گروه کالا")]
        public int ProductGroupID { get; set; }

        [Display(Name = "نام کالا")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [StringLength(100, ErrorMessage = "طول {0} میبایست بین {2} و {1} کاراکتر باشد", MinimumLength = 3)]
        public string ProductTitle { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "توضیحات کوتاه کالا")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string ProductShortDescription { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "توضیحات کامل کالا")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string ProductLongDescription { get; set; }

        [Display(Name = "قیمت کالا")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [DisplayFormat(ApplyFormatInEditMode = false, DataFormatString = "{0:#,0 ریال}")]
        //[UIHint("Tooman")]
        public int ProductPrice { get; set; }

        [Display(Name = "تاریخ آخرین تغییر کالا")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime? ProductLastModificationDate { get; set; } = DateTime.Now;


        [Display(Name = "نمایش کالا")]
        public bool ProductIsActive { get; set; } = true;

        [Display(Name = "تگ های متا کالا")]
        public string? ProductMetaTags { get; set; }

        [Display(Name = "شرح متا کالا")]
        public string? ProductMetaDescription { get; set; }




        // Navigation Properties
        [ForeignKey("ProductGroupID")]
        [Display(Name = "گروه کالا")]
        public virtual ProductGroup ProductGroup { get; set; }

        public virtual List<ProductImage> ProductImages { get; set; }

        public virtual List<OrderDetail> OrderDetails { get; set; }

        public virtual List<Wishlist> Wishlists { get; set; }
        public virtual List<InventoryAction> InventoryActions { get; set; }
        public virtual List<Video> Videos { get; set; }
        public virtual List<ProductTag> ProductTags { get; set; } = new List<ProductTag>();



    }
}
