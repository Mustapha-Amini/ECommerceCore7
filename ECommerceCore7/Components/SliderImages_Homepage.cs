using ECommerceCore7.Data;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceCore7.Components
{
    public class SliderImages_Homepage : ViewComponent
    {
        private ApplicationDbContext db;
        public SliderImages_Homepage(ApplicationDbContext context)
        {
            db = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var sliderImages = db.SliderImages.Where(sl => sl.SliderImageEnabled).ToList();
            return await Task.FromResult((IViewComponentResult)View("SliderImages_Horizontal", sliderImages));
        }
    }
}
