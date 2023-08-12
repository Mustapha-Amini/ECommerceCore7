using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceCore7.Models.Entities
{
    public class VideoType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int VideoTypeID { get; set; }


        [Display(Name = "محل نمایش ویدئو")]
        public string VideoTypeTitle { get; set; }

        //Navigation Prpperties
        public virtual List<Video> Videos { get; set; }
    }
}
