﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerceCore7.Areas.Identity.Models
{
    public class ManageIndexModel
    {
        public string StatusMessage { get; set; }

        [Display(Name = "نام کاربر")]
        public string Username { get; set; }

        [Phone]
        [Display(Name = "شماره موبایل")]
        public string PhoneNumber { get; set; }
    }
}
