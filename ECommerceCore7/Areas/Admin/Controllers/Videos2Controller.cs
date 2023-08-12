using ECommerceCore7.Data;
using ECommerceCore7.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerceCore7.Areas.Admin.Controllers
{
    [Area("Admin")]
    [ControllerTitle("ویدئوها", "ویدئو")]
    public class Videos2Controller : Controller
    {
        private readonly ApplicationDbContext db;
        private string videoSavePath = "\\Uploads\\Videos";

        public Videos2Controller(ApplicationDbContext context)
        {
            db = context;
        }

        // GET: Admin/Videos2
        [Title("فهرست")]
        [Icon("fa-duotone fa-file-lines")]
        public async Task<IActionResult> Index(int? PageID, int? ProductID)
        {
            var model = await (db.Videos
                .Include(v => v.Page)
                .Include(v => v.Product)
                .Include(v => v.VideoType))
                .Where(v =>
                    (PageID == null || v.PageID == PageID.Value)
                     && (ProductID == null || ProductID.Value == ProductID.Value))
                .ToListAsync();
            return View(model);
        }


        // GET: Admin/Videos2/Create
        [Title("ایجاد")]
        [Icon("fa-duotone fa-file-circle-info")]
        public IActionResult Create(int? id)
        {
            ViewData["PageID"] = new SelectList(db.Pages, "PageID", "PageTitle");
            ViewData["ProductID"] = new SelectList(db.Products, "ProductID", "ProductTitle");
            ViewData["VideoTypeID"] = new SelectList(db.VideoTypes, "VideoTypeID", "VideoTypeTitle");
            return View();
        }

        // POST: Admin/Videos2/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Title("ایجاد")]
        [Icon("fa-duotone fa-file-circle-info")]
        public async Task<IActionResult> Create(
            [Bind("VideoID,VideoTypeID,VideoName,ProductID,PageID,VideoDate,VideoComments")]
            Video video, IFormFile VideoFilenameUpload)
        {
            if (ModelState.IsValid)
            {
                string FileName = Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(VideoFilenameUpload.FileName);
                string SavePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot" + videoSavePath, FileName);
                using (var stream = System.IO.File.Create(SavePath))
                {
                    VideoFilenameUpload.CopyTo(stream);
                }
                video.VideoName = FileName;

                db.Add(video);
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PageID"] = new SelectList(db.Pages, "PageID", "PageTitle", video.PageID);
            ViewData["ProductID"] = new SelectList(db.Products, "ProductID", "ProductTitle", video.ProductID);
            ViewData["VideoTypeID"] = new SelectList(db.VideoTypes, "VideoTypeID", "VideoTypeTitle", video.VideoTypeID);
            return View(video);
        }

        // GET: Admin/Videos2/Edit/5


        // GET: Admin/Videos2/Delete/5
        [Title("حذف")]
        [Icon("fa-duotone fa-file-xmark")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || db.Videos == null)
            {
                return NotFound();
            }

            var video = await db.Videos
                .Include(v => v.Page)
                .Include(v => v.Product)
                .Include(v => v.VideoType)
                .FirstOrDefaultAsync(m => m.VideoID == id);
            if (video == null)
            {
                return NotFound();
            }

            return View(video);
        }

        // POST: Admin/Videos2/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Title("حذف")]
        [Icon("fa-duotone fa-file-xmark")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (db.Videos == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Videos'  is null.");
            }
            var video = await db.Videos.FindAsync(id);
            if (video != null)
            {
                string oldSavePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot" + videoSavePath, video.VideoName);
                System.IO.File.Delete(oldSavePath);


                db.Videos.Remove(video);
            }

            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VideoExists(int id)
        {
            return db.Videos.Any(e => e.VideoID == id);
        }
    }
}
