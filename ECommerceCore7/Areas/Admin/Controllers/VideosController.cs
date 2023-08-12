using ECommerceCore7.Data;
using ECommerceCore7.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerceCore7.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    [ControllerTitle("ویدئوها", "ویدئو")]
    public class VideosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private IWebHostEnvironment Environment;
        private string videoSavePath = "Videos";

        public VideosController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            Environment = environment;
        }

        // GET: Admin/Videos
        [Title("فهرست")]
        [Icon("fa-duotone fa-file-lines")]
        public async Task<IActionResult> Index(int? PageID, int? ProductID)
        {

            var model =
                await (_context.Videos
                .Include(v => v.Page)
                .Include(v => v.Product)
                .Include(v => v.VideoType))
                .Where(v =>
                    (PageID == null || v.PageID == PageID.Value)
                     && (ProductID == null || ProductID.Value == ProductID.Value))
                .ToListAsync();
            return View(model);
        }


        // GET: Admin/Videos/Create
        [Title("ایجاد")]
        [Icon("fa-duotone fa-file-circle-info")]
        public IActionResult Create(int? id)
        {

            ViewData["PageID"] = new SelectList(_context.Pages, "PageID", "PageTitle");
            ViewData["ProductID"] = new SelectList(_context.Products, "ProductID", "ProductTitle");
            ViewData["VideoTypeID"] = new SelectList(_context.Set<VideoType>(), "VideoTypeID", "VideoTypeTitle");
            if (id == null)
            {
                return NotFound();
            }


            var model = new Video()
            {
                VideoDate = DateTime.Now,
                ProductID = id.Value,
                PageID = id.Value,
            };

            return View();
        }

        // POST: Admin/Videos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Title("ایجاد")]
        [Icon("fa-duotone fa-file-circle-info")]
        public async Task<IActionResult> Create(
            [Bind("VideoID,VideoName,VideoTypeID,ProductID,PageID,VideoDate,VideoFilename,VideoComments")] Video video,
            IFormFile? VideoFilenameUpload)

        {

            if (ModelState.IsValid)
            {
                string videoPath = Path.Combine(Environment.WebRootPath, "Uploads", "Videos", videoSavePath);
                string FileName = Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(VideoFilenameUpload.FileName);
                string SavePath = Path.Combine(videoPath, FileName);
                using (var stream = System.IO.File.Create(SavePath))
                {
                    VideoFilenameUpload.CopyTo(stream);
                }
                video.VideoName = FileName;


                _context.Add(video);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PageID"] = new SelectList(_context.Pages, "PageID", "PageTitle", video.PageID);
            ViewData["ProductID"] = new SelectList(_context.Products, "ProductID", "ProductTitle", video.ProductID);
            //ViewData["VideoTypeID"] = new SelectList(db.Set<VideoType>(), "VideoTypeID", "VideoTypeTitle", video.VideoTypeID);
            return View(video);
        }

        // GET: Admin/Videos/Edit/5


        // GET: Admin/Videos/Delete/5
        [Title("حذف")]
        [Icon("fa-duotone fa-file-xmark")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Videos == null)
            {
                return NotFound();
            }

            var video = await _context.Videos
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

        // POST: Admin/Videos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Title("حذف")]
        [Icon("fa-duotone fa-file-xmark")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Videos == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Video'  is null.");
            }
            var video = await _context.Videos.FindAsync(id);
            if (video != null)
            {
                _context.Videos.Remove(video);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VideoExists(int id)
        {
            return _context.Videos.Any(e => e.VideoID == id);
        }
    }
}
