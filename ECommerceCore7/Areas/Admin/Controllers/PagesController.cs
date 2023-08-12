using ECommerceCore7.Data;
using ECommerceCore7.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Data;
using System.Security.Claims;

namespace eShopCore7.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    [ControllerTitle("صفحات", "صفحه")]
    public class PagesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private IWebHostEnvironment Environment;
        private string imageSavePath = "Pages";

        public PagesController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            Environment = environment;
        }

        // GET: Admin/Pages
        [Title("فهرست")]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Pages.Include(p => p.PageGroup);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Admin/Pages/Details/5
        [Title("جزئیات")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Pages == null)
            {
                return NotFound();
            }

            var page = await _context.Pages
                .Include(p => p.PageGroup)
                .FirstOrDefaultAsync(m => m.PageID == id);
            if (page == null)
            {
                return NotFound();
            }

            return View(page);
        }

        // GET: Admin/Pages/Create
        [Title("افزودن")]
        public IActionResult Create()
        {
            //ViewData["PageGroupID"] 
            ViewBag.PageGroupID =
                new SelectList(_context.PageGroups, "PageGroupID", "PageGroupTitle");


            ViewBag.UserID =
                new SelectList(_context.Users, "Id", "UserName");

            ViewBag.Tags = new SelectList(_context.Tags, "TagID", "TagTitle");

            return View();
        }

        // POST: Admin/Pages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Title("افزودن")]
        public async Task<IActionResult> Create(
            Page page,
            IFormFile? PageImageFilenameUpload,
            List<int> Tags)
        {
            // Get Logged In User ID
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var UserID = Convert.ToInt32(userIdString);

            page.PageLastModified = DateTime.Now;
            //var sanitizer = new HtmlSanitizer();
            //page.PageShortContent = sanitizer.Sanitize(page.PageShortContent);
            //page.PageLongContent = sanitizer.Sanitize(page.PageLongContent);
            //page.PageMetaDescription = sanitizer.Sanitize(page.PageMetaDescription);

            page.UserID = UserID;
            if (ModelState.IsValid)
            {
                if (PageImageFilenameUpload != null)
                {
                    string imagesPath = Path.Combine(Environment.WebRootPath, "Uploads", "Images", imageSavePath);
                    string FileName = Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(PageImageFilenameUpload.FileName);
                    string SavePath = Path.Combine(imagesPath, FileName);
                    using (var stream = System.IO.File.Create(SavePath))
                    {
                        PageImageFilenameUpload.CopyTo(stream);
                    }
                    page.PageImageFilename = FileName;
                }
                #region Add Tags To Page
                if (Tags != null && Tags.Any())
                {
                    foreach (var tag in Tags)
                    {
                        page.PageTags.Add(new PageTag()
                        {
                            TagID = tag
                        });
                    }
                }
                #endregion

                _context.Add(page);
                await _context.SaveChangesAsync();
                RebuildStaticPagesAsJson();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PageGroupID"] = new SelectList(_context.PageGroups, "PageGroupID", "PageGroupTitle", page.PageGroupID);
            ViewData["UserID"] = new SelectList(_context.Users, "Id", "UserName", page.UserID);

            var dbTags = _context.Tags.ToList();
            ViewBag.Tags = dbTags.Select(t => new SelectListItem()
            {
                Text = t.TagTitle,
                Value = t.TagID.ToString(),
                Selected = Tags?.Contains(t.TagID) ?? false
            })?.ToList();

            //return View("FolanView",page);
            return View(page);
        }

        // GET: Admin/Pages/Edit/5
        [Title("ویرایش")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Pages == null)
            {
                return NotFound();
            }

            var page = await _context.Pages.FindAsync(id);
            if (page == null)
            {
                return NotFound();
            }
            ViewData["PageGroupID"] =
                new SelectList(_context.PageGroups, "PageGroupID", "PageGroupTitle", page.PageGroupID);
            ViewData["UserID"] = new SelectList(_context.Users, "Id", "UserName", page.UserID);

            #region Tags Handling
            var pageTags = _context.PageTags.Where(pt => pt.PageID == page.PageID)
                .Select(at => at.TagID)
                .ToList();

            ViewBag.Tags = _context.Tags.Select(t => new SelectListItem()
            {
                Text = t.TagTitle,
                Value = t.TagID.ToString(),
                Selected = pageTags.Contains(t.TagID)
            }).ToList();

            #endregion
            return View(page);
        }

        // POST: Admin/Pages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Title("ویرایش")]
        public async Task<IActionResult> Edit(int id,
            Page page,
            IFormFile? PageImageFilenameUpload,
            List<int> Tags)
        {
            if (id != page.PageID)
            {
                return NotFound();
            }

            page.PageLastModified = DateTime.Now;

            //var sanitizer = new HtmlSanitizer();
            //page.PageShortContent = sanitizer.Sanitize(page.PageShortContent);
            //page.PageLongContent = sanitizer.Sanitize(page.PageLongContent);
            //page.PageMetaDescription = sanitizer.Sanitize(page.PageMetaDescription);

            if (ModelState.IsValid)
            {
                try
                {
                    if (PageImageFilenameUpload != null)
                    {
                        string FileName = Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(PageImageFilenameUpload.FileName);
                        string imagesPath = Path.Combine(Environment.WebRootPath, "Uploads", "Images", imageSavePath);
                        string SavePath = Path.Combine(imagesPath, FileName);

                        // Maybe Page had not any images before, so delete image only if already exists:
                        if (!string.IsNullOrEmpty(page.PageImageFilename))
                        {
                            string oldSavePath = Path.Combine(imagesPath, page.PageImageFilename);
                            System.IO.File.Delete(oldSavePath);
                        }

                        using (var stream = System.IO.File.Create(SavePath))
                        {
                            PageImageFilenameUpload.CopyTo(stream);
                        }
                        page.PageImageFilename = FileName;
                    }

                    if (Tags != null && Tags.Any())
                    {
                        var existingTags = _context.PageTags.Where(pt => pt.PageID == page.PageID).ToList();
                        _context.PageTags.RemoveRange(existingTags);

                        foreach (var tag in Tags)
                        {
                            page.PageTags.Add(new PageTag()
                            {
                                TagID = tag
                            });
                        }
                    }


                    _context.Update(page);
                    await _context.SaveChangesAsync();
                    RebuildStaticPagesAsJson();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PageExists(page.PageID))
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
            ViewData["PageGroupID"] = new SelectList(_context.PageGroups, "PageGroupID", "PageGroupTitle", page.PageGroupID);
            ViewData["UserID"] = new SelectList(_context.Users, "Id", "UserName", page.UserID);
            return View(page);
        }

        // GET: Admin/Pages/Delete/5
        [Title("حذف")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Pages == null)
            {
                return NotFound();
            }

            var page = await _context.Pages
                .Include(p => p.PageGroup)
                .FirstOrDefaultAsync(m => m.PageID == id);
            if (page == null)
            {
                return NotFound();
            }

            return View(page);
        }

        // POST: Admin/Pages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Title("حذف")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Pages == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Pages'  is null.");
            }
            var page = await _context.Pages.FindAsync(id);
            if (page != null)
            {
                string imagesPath = Path.Combine(Environment.WebRootPath, "Uploads", "Images", imageSavePath);
                string imageFilePath = Path.Combine(imagesPath, page.PageImageFilename);
                if (System.IO.File.Exists(imageFilePath))
                {
                    System.IO.File.Delete(imageFilePath);
                }
                _context.Pages.Remove(page);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PageExists(int id)
        {
            return _context.Pages.Any(e => e.PageID == id);
        }

        private void RebuildStaticPagesAsJson()
        {
            var pagesWithRoute =
                _context.Pages.Include(p => p.PageGroup)
                    .Where(p => !string.IsNullOrEmpty(p.PageRoute) && p.PageEnabled)
                    .ToList();

            string cacheFileName = Path.Combine(Environment.ContentRootPath, "staticPages.json"); ;

            var jsonConverted = Newtonsoft.Json.JsonConvert
                .SerializeObject(pagesWithRoute,
                    Formatting.Indented,
                new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                }
            );
            System.IO.File.WriteAllText(cacheFileName, jsonConverted);
        }

        public async Task<IActionResult> GetTags(string prefix = "")
        {
            var tags =
                await _context.Tags.Where(t => t.TagTitle.Contains(prefix) || prefix == "")
                    .Select(t => t.TagTitle)
                    .ToArrayAsync();
            return Json(tags);
        }

        public async Task<IActionResult> SaveTag(string tag = "")
        {
            if (string.IsNullOrEmpty(tag))
            {
                return Json(new { ID = -1 });
            }
            // Check if tag is duplicate 
            var duplicateTag = await _context.Tags.FirstOrDefaultAsync(t => t.TagTitle == tag);
            if (duplicateTag != null)
            {
                return Json(new { ID = -2 });
            }
            var newTag = new Tag()
            {
                TagTitle = tag
            };
            _context.Tags.Add(newTag);
            await _context.SaveChangesAsync();
            return Json(new { ID = newTag.TagID });
        }
    }
}
