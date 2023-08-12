using ECommerceCore7.Models.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerceCore7.Areas.Identity.Models
{
    public class UserLoginWithSms
    {
        [Key]
        public int LoginWithSmsID { get; set; }

        [Required]
        [StringLength(20)]
        public string PhoneNumber { get; set; }

        [Required]
        [StringLength(10)]
        public string AuthenticationCode { get; set; }

        public string AuthenticationKey { get; set; }

        public DateTime ExpireDate { get; set; }


        public int UserID { get; set; }

        [ForeignKey("UserID")]
        public virtual ApplicationUser User { get; set; }

        // Methods:
        public string GenerateNewAuthenticationCode()
        {
            Random rnd = new Random();
            return rnd.Next(1000000, 9999999).ToString();
        }

        public string GenerateNewAuthenticationKey()
        {
            return Guid.NewGuid().ToString("N");
        }

        public void Initialize()
        {
            AuthenticationCode = GenerateNewAuthenticationCode();
            AuthenticationKey = GenerateNewAuthenticationKey();
        }
    }

}
