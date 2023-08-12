using ECommerceCore7.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.TagHelpers.Cache;
using Microsoft.EntityFrameworkCore;

namespace ECommerceCore7.Components
{
    public class LatestPagesViewComponent : ViewComponent
    {
        private ApplicationDbContext db;
        public LatestPagesViewComponent(ApplicationDbContext context)
        {
            db = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(int PageCount = 1, bool GenerateForSideBar = true)
        {
            var pages =
                db.Pages
                     .Include(p => p.User)
                     .Where(p => string.IsNullOrEmpty(p.PageRoute))
                    .OrderByDescending(pg => pg.PageLastModified)
                    .Take(PageCount)
                    .ToList();
            string viewName = GenerateForSideBar ? "LatestPages_Sidebar" : "LatestPages_Horizontal";
            return await Task.FromResult((IViewComponentResult)View(viewName, pages));
        }
    }
}
