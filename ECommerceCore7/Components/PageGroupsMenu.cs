using ECommerceCore7.Data;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceCore7.Components
{
    public class PageGroupsMenu : ViewComponent
    {
        private ApplicationDbContext db;
        public PageGroupsMenu(ApplicationDbContext context)
        {
            db = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(bool IsMobileMenu = false)
        {
            var pageGroups = db.PageGroups
                .Where(pg => pg.Pages.Any(p => string.IsNullOrEmpty(p.PageRoute)))
                .ToList();

            string viewName = IsMobileMenu ? "PageGroupsMobileMenu" : "PageGroupsTopMenu";

            return await Task.FromResult((IViewComponentResult)View(viewName, pageGroups));
        }
    }
}
