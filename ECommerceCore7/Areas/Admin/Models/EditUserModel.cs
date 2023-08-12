using System.ComponentModel.DataAnnotations;

namespace ECommerceCore7.Areas.Admin.Models
{
    public class EditUserModel
    {
        public int UserID { get; set; }

        [Display(Name = "نام کاربر")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string Username { get; set; }
    }
}