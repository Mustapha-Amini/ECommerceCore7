using ECommerceCore7.Models.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceCore7.Models.Entities
{
    public class Order
    {
        //[DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Key]
        public int OrderID { get; set; }

        [Display(Name = "کاربر")]
        public int UserID { get; set; }

        [Display(Name = "تاریخ فاکتور")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime OrderDate { get; set; }

        [Display(Name = "وضعیت فاکتور")]
        public bool IsFinalized { get; set; }


        // Navigation Properties
        [ForeignKey("UserID")]
        public virtual ApplicationUser User { get; set; }

        public virtual List<OrderDetail> OrderDetails { get; set; }

        public virtual List<InventoryAction> InventoryActions { get; set; }

    }
}
