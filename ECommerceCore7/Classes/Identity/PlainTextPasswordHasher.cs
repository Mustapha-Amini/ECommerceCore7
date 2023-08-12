using ECommerceCore7.Models.Identity;
using Microsoft.AspNetCore.Identity;
using System.Text.RegularExpressions;

namespace ECommerceCore7.Classes.Identity
{
    public class PlainTextPasswordHasher<TUser> : PasswordHasher<TUser> where TUser : class
    {
        public override string HashPassword(TUser user, string password)
        {
            return password;
        }

        public override PasswordVerificationResult VerifyHashedPassword(TUser user, string hashedPassword, string providedPassword)
        {
            return hashedPassword == providedPassword
                ? PasswordVerificationResult.Success
                : PasswordVerificationResult.Failed;
        }
    }


}
