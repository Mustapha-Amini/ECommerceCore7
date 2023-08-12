using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceCore7.Models.Entities
{
    public class Video
    {
        [Key]
        public int VideoID { get; set; }


        [Display(Name = "محل نمایش ویدئو")]
        public int VideoTypeID { get; set; }


        [Display(Name = "نام ویدئو")]
        public string? VideoName { get; set; }

        [Display(Name = "کالا")]
        public int? ProductID { get; set; }

        [Display(Name = "صفحه")]
        public int? PageID { get; set; }

        [Display(Name = "تاریخ")]
        public DateTime VideoDate { get; set; }

        [Display(Name = "نظرات")]
        public string? VideoComments { get; set; }

        //Navigation Properties
        [ForeignKey("VideoTypeID")]
        [Display(Name = "محل نمایش ویدئو")]
        public virtual VideoType VideoType { get; set; }

        [ForeignKey("ProductID")]
        [Display(Name = "محصول")]
        public virtual Product? Product { get; set; }

        [ForeignKey("PageID")]
        [Display(Name = "صفحه")]
        public virtual Page? Page { get; set; }
    }
}
