using ECommerceCore7.Data;
using ECommerceCore7.Models.Entities;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceCore7.Components
{
    public class StaticPagesMenu : ViewComponent
    {
        private ApplicationDbContext db;
        private IWebHostEnvironment Environment;
        public StaticPagesMenu(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            db = context;
            Environment = environment;
        }

        public async Task<IViewComponentResult> InvokeAsync(bool GenerateMenuForFooter = false, bool IsMobileMenu = false)
        {
            string viewName = string.Empty;
            List<Page> pages = new List<Page>();
            string cacheFileName = Path.Combine(Environment.ContentRootPath, "staticPages.json"); ;
            if (System.IO.File.Exists(cacheFileName))
            {
                var cacheFileContents = System.IO.File.ReadAllText(cacheFileName);
                pages = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Page>>(cacheFileContents);
            }

            if (IsMobileMenu)
            {
                viewName = "StaticPagesMobileMenu";
            }
            else
            {
                viewName = GenerateMenuForFooter ? "StaticPagesFooterMenu" : "StaticPagesTopMenu";
            }

            return await Task.FromResult((IViewComponentResult)View(viewName, pages));
        }
    }
}

