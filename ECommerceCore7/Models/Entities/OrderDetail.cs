using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceCore7.Models.Entities
{
    public class OrderDetail
    {
        [Key]
        public int OrderDetailID { get; set; }

        [Display(Name = "فاکتور")]
        public int OrderID { get; set; }

        [Display(Name = "کالا")]
        public int ProductID { get; set; }

        [Display(Name = "تعداد کالا")]
        public int ProductCount { get; set; }

        // Navigation Properties
        [Display(Name = "فاکتور")]
        [ForeignKey("OrderID")]
        public virtual Order? Order { get; set; }


        [Display(Name = "کالا")]
        [ForeignKey("ProductID")]
        public virtual Product? Product { get; set; }






    }
}
