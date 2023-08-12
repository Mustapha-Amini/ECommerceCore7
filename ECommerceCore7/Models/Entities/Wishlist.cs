using ECommerceCore7.Models.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceCore7.Models.Entities
{
    public class Wishlist
    {
        [Key]
        public int WishlistID { get; set; }

        [Display(Name = "کاربر")]
        public int UserID { get; set; }

        [Display(Name = "کالا")]
        public int ProductID { get; set; }

        [Display(Name = "تاریخ افزوده شدن کالا به لیست")]
        public DateTime AddedToListDate { get; set; }

        // Navigation Properties
        [ForeignKey("UserID")]
        [Display(Name = "کاربر")]
        public ApplicationUser User { get; set; }


        [ForeignKey("ProductID")]
        [Display(Name = "کالا")]
        public Product Product { get; set; }
    }
}
