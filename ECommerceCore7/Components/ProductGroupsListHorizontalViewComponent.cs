using ECommerceCore7.Data;
using ECommerceCore7.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerceCore7.Components
{
    public class ProductGroupsListHorizontalViewComponent : ViewComponent
    {
        private ApplicationDbContext db;
        public ProductGroupsListHorizontalViewComponent(ApplicationDbContext context)
        {
            db = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var productGroups = db.ProductGroups.Include(pg => pg.Products)
                .Where(pg => pg.Products.Any(p => p.ProductIsActive))
                .ToList();

            return await Task.FromResult((IViewComponentResult)View("ProductGroupsListHorizontal", productGroups));
        }
    }
}