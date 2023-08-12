using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceCore7.Models.Entities
{
    public class ProductImage
    {
        [Key]
        public int ProductImageID { get; set; }

        [Display(Name = "کالا")]
        public int ProductID { get; set; }

        [Display(Name = "تصویر کالا")]
        public string? ProductImageFilename { get; set; }

        // Navigation Properties
        [ForeignKey("ProductID")]
        [Display(Name = "تصویر کالا")]
        public virtual Product? Product { get; set; }

    }
}
