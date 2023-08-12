using Microsoft.AspNetCore.Identity;

namespace ECommerceCore7.Models.Identity
{
    public class ApplicationRoleClaim : IdentityRoleClaim<int>
    {
        public virtual ApplicationRole Role { get; set; }
    }
}