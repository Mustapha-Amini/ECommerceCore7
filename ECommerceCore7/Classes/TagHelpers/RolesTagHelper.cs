using Azure.Core;
using ECommerceCore7.Data;
using ECommerceCore7.Models.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Policy;
using System.Threading.Tasks;

namespace ECommerceCore7.Classes.TagHelpers
{
    [HtmlTargetElement("*", Attributes = RolesAttributeName)]
    public class RolesTagHelper : TagHelper
    {
        private const string RolesAttributeName = "visible-to-roles";

        private readonly UserManager<ApplicationUser> userManager;
        private readonly IHttpContextAccessor httpContextAccessor;


        public RolesTagHelper(
            UserManager<ApplicationUser> userManager,
            IHttpContextAccessor httpContextAccessor)
        {
            this.userManager = userManager;
            this.httpContextAccessor = httpContextAccessor;
        }

        [HtmlAttributeName(RolesAttributeName)]
        public string Roles { get; set; }

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var httpContext = httpContextAccessor.HttpContext;
            var userIsAuthenticated = httpContext.User.Identity.IsAuthenticated;
            if (!userIsAuthenticated)
            {
                output.SuppressOutput();
            }
            else
            {
                var allowdRolesList = Roles.Split(",").ToList();
                var UserID = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);// will give the user's id
                var UserName = httpContext.User.FindFirstValue(ClaimTypes.Name); // will give the user's userName
                var user = await userManager.FindByIdAsync(UserID);
                var userRoles = await userManager.GetRolesAsync(user);

                var userHasAnyOfAllowdRoles = allowdRolesList.Intersect(userRoles).Any();
                if (!userHasAnyOfAllowdRoles)
                {
                    output.SuppressOutput();
                }
            }
        }
    }
}
