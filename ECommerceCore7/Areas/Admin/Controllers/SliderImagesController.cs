using ECommerceCore7.Data;
using ECommerceCore7.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerceCore7.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    [ControllerTitle("اسلایدرها", "اسلایدر")]
    public class SliderImagesController : Controller
    {
        private readonly ApplicationDbContext db;
        private IWebHostEnvironment Environment;
        private string imageSavePath = "SliderImages";

        public SliderImagesController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            db = context;
            Environment = environment;
        }

        // GET: Admin/SliderImages
        [Title("فهرست")]
        [Icon("fa-duotone fa-file-lines")]
        public async Task<IActionResult> Index()
        {
            return View(await db.SliderImages.ToListAsync());
        }


        [Title("جزئیات")]
        [Icon("fa-duotone fa-file-circle-info")]
        // GET: Admin/SliderImages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || db.SliderImages == null)
            {
                return NotFound();
            }

            var sliderImage = await db.SliderImages
                .FirstOrDefaultAsync(m => m.SliderImageID == id);
            if (sliderImage == null)
            {
                return NotFound();
            }

            return View(sliderImage);
        }

        // GET: Admin/SliderImages/Create
        [Title("ایجاد")]
        [Icon("fa-duotone fa-file-circle-info")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/SliderImages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Title("ایجاد")]
        [Icon("fa-duotone fa-file-circle-info")]
        public async Task<IActionResult> Create(
            [Bind("SliderImageID,SlideImageMainTitle,SlideImageShortTitle,SlideImageShortDescription,SlideImageFilename,SliderImageEnabled")]
            SliderImage sliderImage
            , IFormFile SlideImageFilenameUpload)
        {
            if (ModelState.IsValid)
            {
                string FileName = Guid.NewGuid().ToString().Replace("-", "")
                                  + Path.GetExtension(SlideImageFilenameUpload.FileName);
                string imagesPath = Path.Combine(Environment.WebRootPath, "Uploads", "Images", imageSavePath);
                string SavePath = Path.Combine(imagesPath, FileName);
                using (var stream = System.IO.File.Create(SavePath))
                {
                    SlideImageFilenameUpload.CopyTo(stream);
                }
                sliderImage.SlideImageFilename = FileName;

                db.Add(sliderImage);
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(sliderImage);
        }

        // GET: Admin/SliderImages/Edit/5
        [Title("ویرایش")]
        [Icon("fa-duotone fa-file-pen")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || db.SliderImages == null)
            {
                return NotFound();
            }

            var sliderImage = await db.SliderImages.FindAsync(id);
            if (sliderImage == null)
            {
                return NotFound();
            }
            return View(sliderImage);
        }

        // POST: Admin/SliderImages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Title("ویرایش")]
        [Icon("fa-duotone fa-file-pen")]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("SliderImageID,SlideImageMainTitle,SlideImageShortTitle,SlideImageShortDescription,SlideImageFilename,SliderImageEnabled")]
            SliderImage sliderImage
            , IFormFile SlideImageFilenameUpload)
        {
            if (id != sliderImage.SliderImageID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (SlideImageFilenameUpload != null)
                    {
                        string FileName = Guid.NewGuid().ToString().Replace("-", "")
                                          + Path.GetExtension(SlideImageFilenameUpload.FileName);
                        string imagesPath = Path.Combine(Environment.WebRootPath, "Uploads", "Images", imageSavePath);
                        string SavePath = Path.Combine(imagesPath, FileName);
                        string oldSavePath = Path.Combine(imagesPath, sliderImage.SlideImageFilename);

                        using (var stream = System.IO.File.Create(SavePath))
                        {
                            SlideImageFilenameUpload.CopyTo(stream);
                        }
                        System.IO.File.Delete(oldSavePath);
                        sliderImage.SlideImageFilename = FileName;
                    }
                    db.Update(sliderImage);
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SliderImageExists(sliderImage.SliderImageID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(sliderImage);
        }

        // GET: Admin/SliderImages/Delete/5
        [Title("حذف")]
        [Icon("fa-duotone fa-file-xmark")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || db.SliderImages == null)
            {
                return NotFound();
            }

            var sliderImage = await db.SliderImages
                .FirstOrDefaultAsync(m => m.SliderImageID == id);
            if (sliderImage == null)
            {
                return NotFound();
            }

            return View(sliderImage);
        }

        // POST: Admin/SliderImages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Title("حذف")]
        [Icon("fa-duotone fa-file-xmark")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (db.SliderImages == null)
            {
                return Problem("Entity set 'ApplicationDbContext.SliderImage'  is null.");
            }
            var sliderImage = await db.SliderImages.FindAsync(id);
            if (sliderImage != null)
            {
                string imagesPath = Path.Combine(Environment.WebRootPath, "Uploads", "Images", imageSavePath);
                string oldSavePath = Path.Combine(imagesPath, sliderImage.SlideImageFilename);
                System.IO.File.Delete(oldSavePath);

                db.SliderImages.Remove(sliderImage);
            }

            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SliderImageExists(int id)
        {
            return db.SliderImages.Any(e => e.SliderImageID == id);
        }
    }
}
