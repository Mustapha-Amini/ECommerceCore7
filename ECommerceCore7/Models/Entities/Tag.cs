using System.ComponentModel.DataAnnotations;

namespace ECommerceCore7.Models.Entities
{
    public class Tag
    {
        [Key]
        public int TagID { get; set; }

        [Display(Name = "عنوان تگ")]
        public string TagTitle { get; set; }

        // Navigation Properties
        public virtual List<PageTag> PageTags { get; set; }

        public virtual List<ProductTag> ProductTags { get; set; }
    }
}
