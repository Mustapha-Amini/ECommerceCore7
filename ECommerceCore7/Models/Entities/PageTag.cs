using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceCore7.Models.Entities
{
    public class PageTag
    {
        [Key]
        public int PageTagID { get; set; }

        public int TagID { get; set; }
        public int PageID { get; set; }

        // Navigation Properties
        [ForeignKey("TagID")]
        public virtual Tag Tag { get; set; }

        [ForeignKey("PageID")]
        public virtual Page Page { get; set; }
    }
}
