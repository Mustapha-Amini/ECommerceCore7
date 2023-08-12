﻿using System.ComponentModel.DataAnnotations;

namespace ECommerceCore7.Areas.Identity.Models
{
    public class AddPhoneNumberModel
    {
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Display(Name = "شماره موبایل")]
        public string PhoneNumber { get; set; }
    }
}