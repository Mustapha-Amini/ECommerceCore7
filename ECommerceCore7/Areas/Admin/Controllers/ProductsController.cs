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
    [ControllerTitle("کالاها", "کالا")]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private IWebHostEnvironment Environment;
        private string imageSavePath = "Products";
        public ProductsController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            Environment = environment;
        }

        // GET: Admin/Products
        [Title("فهرست")]
        [Icon("fa-solid fa-list")]
        public async Task<IActionResult> Index(int? id)
        {
            var model =
                _context.Products
                    .Include(p => p.ProductGroup)
                    .Where(p => id == null || p.ProductGroupID == id.Value)
                    .ToList();
            return View(model);
        }

        // GET: Admin/Products/Details/5
        [Title("جزئیات")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.ProductGroup)
                .FirstOrDefaultAsync(m => m.ProductID == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Admin/Products/Create
        [Title("افزودن")]
        public IActionResult Create()
        {
            ViewData["ProductGroupID"] = new SelectList(_context.ProductGroups, "ProductGroupID", "ProductGroupTitle");
            ViewBag.Tags = new SelectList(_context.Tags, "TagID", "TagTitle");
            return View();
        }

        // POST: Admin/Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Title("افزودن")]
        public async Task<IActionResult> Create(
            Product product,
            List<int> Tags)
        {
            product.ProductLastModificationDate = DateTime.Now;

            if (ModelState.IsValid)
            {

                #region Add Tags To Product
                if (Tags != null && Tags.Any())
                {
                    foreach (var tag in Tags)
                    {
                        product.ProductTags.Add(new ProductTag()
                        {
                            TagID = tag
                        });
                    }
                }
                #endregion
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductGroupID"] = new SelectList(_context.ProductGroups, "ProductGroupID", "ProductGroupTitle", product.ProductGroupID);
            var dbTags = _context.Tags.ToList();
            ViewBag.Tags = dbTags.Select(t => new SelectListItem()
            {
                Text = t.TagTitle,
                Value = t.TagID.ToString(),
                Selected = Tags?.Contains(t.TagID) ?? false
            })?.ToList();

            return View(product);
        }

        // GET: Admin/Products/Edit/5
        [Title("ویرایش")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["ProductGroupID"] = new SelectList(_context.ProductGroups, "ProductGroupID", "ProductGroupTitle", product.ProductGroupID);
            #region Tags Handling
            var productTags = _context.ProductTags.Where(productTags => productTags.ProductID == product.ProductID)
                .Select(at => at.TagID)
                .ToList();

            ViewBag.Tags = _context.Tags.Select(t => new SelectListItem()
            {
                Text = t.TagTitle,
                Value = t.TagID.ToString(),
                Selected = productTags.Contains(t.TagID)
            }).ToList();
            #endregion
            return View(product);
        }

        // POST: Admin/Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Title("ویرایش")]
        public async Task<IActionResult> Edit(
            int id,
            Product product,
            List<int> Tags)
        {
            if (id != product.ProductID)
            {
                return NotFound();
            }
            product.ProductLastModificationDate = DateTime.Now;

            if (ModelState.IsValid)
            {
                try
                {

                    if (Tags != null && Tags.Any())
                    {
                        var existingTags = _context.ProductTags.Where(pt => pt.ProductID == product.ProductID).ToList();
                        _context.ProductTags.RemoveRange(existingTags);

                        foreach (var tag in Tags)
                        {
                            product.ProductTags.Add(new ProductTag()
                            {
                                TagID = tag
                            });
                        }
                    }
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductID))
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
            ViewData["ProductGroupID"] = new SelectList(_context.ProductGroups, "ProductGroupID", "ProductGroupTitle", product.ProductGroupID);
            return View(product);
        }

        // GET: Admin/Products/Delete/5
        [Title("حذف")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.ProductGroup)
                .FirstOrDefaultAsync(m => m.ProductID == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }



        // POST: Admin/Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Title("حذف")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Products == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Products'  is null.");
            }

            //var product = await _context.Products.FindAsync(id);
            var product =
                await _context.Products
                    .Include(p => p.ProductImages)
                    .FirstOrDefaultAsync(p => p.ProductID == id);

            if (product != null)
            {
                if (product.ProductImages.Any())
                {
                    string imagesPath = Path.Combine(Environment.WebRootPath, "Uploads", "Images", imageSavePath);
                    foreach (var image in product.ProductImages)
                    {
                        string filePath = Path.Combine(imagesPath, image.ProductImageFilename);
                        if (System.IO.File.Exists(filePath))
                        {
                            System.IO.File.Delete(filePath);
                        }
                    }
                }
                _context.Products.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return (_context.Products?.Any(e => e.ProductID == id)).GetValueOrDefault();
        }

        public async Task<IActionResult> SetActive(int id, bool IsActive)
        {
            var product = await _context.Products
                .Include(p => p.ProductGroup)
                .FirstOrDefaultAsync(m => m.ProductID == id);
            if (product == null)
            {
                return NotFound();
            }

            product.ProductIsActive = IsActive;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
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
