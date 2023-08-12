using System.ComponentModel.DataAnnotations;

namespace ECommerceCore7.Models.Identity
{
    public class UserPreRegisterationConfirm
    {
        [Display(Name = "کلید شناسایی")]
        public string AuthenticationKey { get; set; }

        [Display(Name = "کد امنیتی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string AuthenticationCode { get; set; }
    }
}
