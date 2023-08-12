using ECommerceCore7.Data;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceCore7.Components
{
    public class ProductGroupsMenu : ViewComponent
    {
        private ApplicationDbContext db;
        public ProductGroupsMenu(ApplicationDbContext context)
        {
            db = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(bool IsMobileMenu = false)
        {
            var productGroups = db.ProductGroups
                .Where(pg => pg.Products.Any(p => p.ProductIsActive))
                .ToList();

            string viewName = IsMobileMenu ? "ProductGroupsMobileMenu" : "ProductGroupsTopMenu";

            return await Task.FromResult((IViewComponentResult)View(viewName, productGroups));
        }
    }
}
