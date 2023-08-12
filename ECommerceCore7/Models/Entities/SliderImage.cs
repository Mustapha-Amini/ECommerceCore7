using System.ComponentModel.DataAnnotations;

namespace ECommerceCore7.Models.Entities
{
    public class SliderImage
    {
        [Key]
        public int SliderImageID { get; set; }

        [Display(Name = "متن بزرگ اسلایدر")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string SlideImageMainTitle { get; set; }

        [Display(Name = "متن کوچک اسلایدر")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string SlideImageShortTitle { get; set; }

        [Display(Name = "شرح خلاصه اسلایدر")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string SlideImageShortDescription { get; set; }

        [Display(Name = "تصویر اسلایدر")]
        public string? SlideImageFilename { get; set; }

        [Display(Name = "فعال باشد؟")]
        public bool SliderImageEnabled { get; set; }
    }
}
