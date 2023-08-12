using Microsoft.AspNetCore.Identity;

namespace ECommerceCore7.Models.Identity
{
    public class ApplicationUserClaim : IdentityUserClaim<int>
    {
        public virtual ApplicationUser User { get; set; }
    }
}