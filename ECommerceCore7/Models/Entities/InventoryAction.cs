using ECommerceCore7.Models.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceCore7.Models.Entities
{
    public class InventoryAction
    {
        [Key]
        public int InventoryActionID { get; set; }

        [Display(Name = "عملیات انبار")]
        public int InventoryActionTypeID { get; set; }

        [Display(Name = "کالا")]
        public int ProductID { get; set; }

        [Display(Name = "کاربر")]
        public int? UserID { get; set; }

        [Display(Name = "سفارش")]
        public int? OrderID { get; set; }

        [Display(Name = "توضیحات")]
        [DataType(DataType.MultilineText)]
        public string InventoryActionComments { get; set; }

        [Display(Name = "تاریخ عملیات")]
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime InventoryActionDate { get; set; }

        [Display(Name = "تعداد گردش کالا")]
        public int InventoryActionCount { get; set; }

        // Navigation Properties
        [ForeignKey("InventoryActionTypeID")]
        [Display(Name = "عملیات انبار")]
        public virtual InventoryActionType InventoryActionType { get; set; }

        [ForeignKey("ProductID")]
        [Display(Name = "کالا")]
        public virtual Product Product { get; set; }

        [ForeignKey("UserID")]
        [Display(Name = "کاربر")]
        public virtual ApplicationUser User { get; set; }

        [ForeignKey("OrderID")]
        [Display(Name = "سفارش")]
        public virtual Order Order { get; set; }
    }
}
