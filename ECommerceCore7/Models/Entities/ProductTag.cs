using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceCore7.Models.Entities
{
    public class ProductTag
    {
        [Key]
        public int ProductTagID { get; set; }

        public int TagID { get; set; }
        public int ProductID { get; set; }

        // Navigation Properties
        [ForeignKey("TagID")]
        public virtual Tag Tag { get; set; }

        [ForeignKey("ProductID")]
        public virtual Product Product { get; set; }
    }
}
