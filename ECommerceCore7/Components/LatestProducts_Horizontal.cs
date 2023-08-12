using ECommerceCore7.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerceCore7.Components
{
    public class LatestProducts_Horizontal : ViewComponent
    {
        private ApplicationDbContext db;
        public LatestProducts_Horizontal(ApplicationDbContext context)
        {
            db = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(int ProductCount = 4)
        {
            var products =
                db.Products
                    .Include(p => p.ProductImages)
                    .OrderByDescending(p => p.ProductLastModificationDate)
                    .Take(ProductCount)
                    .ToList();
            return await Task.FromResult((IViewComponentResult)View("LatestProducts_Horizontal", products));
        }
    }
}
