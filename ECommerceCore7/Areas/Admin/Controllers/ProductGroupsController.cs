using ECommerceCore7.Data;
using ECommerceCore7.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
    [ControllerTitle("گروه های کالاها", "گروه کالا")]
    public class ProductGroupsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private IWebHostEnvironment Environment;
        private string imageSavePath = "ProductGroups";

        public ProductGroupsController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            Environment = environment;
        }

        // GET: Admin/ProductGroups
        [Title("فهرست")]
        public async Task<IActionResult> Index()
        {
            return _context.ProductGroups != null ?
                          View(await _context.ProductGroups.Include(pg => pg.Products).ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.ProductGroups'  is null.");
        }

        // GET: Admin/ProductGroups/Details/5
        [Title("جزئیات")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ProductGroups == null)
            {
                return NotFound();
            }

            var productGroup = await _context.ProductGroups
                .FirstOrDefaultAsync(m => m.ProductGroupID == id);
            if (productGroup == null)
            {
                return NotFound();
            }

            return View(productGroup);
        }

        // GET: Admin/ProductGroups/Create
        [Title("افزودن")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/ProductGroups/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Title("افزودن")]
        public async Task<IActionResult> Create(
             ProductGroup productGroup
            , IFormFile ProductGroupImageFilenameUpload)
        {
            if (ModelState.IsValid)
            {
                string imagesPath = Path.Combine(Environment.WebRootPath, "Uploads", "Images", imageSavePath);
                string FileName = Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(ProductGroupImageFilenameUpload.FileName);
                string SavePath = Path.Combine(imagesPath, FileName);
                using (var stream = System.IO.File.Create(SavePath))
                {
                    ProductGroupImageFilenameUpload.CopyTo(stream);
                }
                productGroup.ProductGroupImageFilename = FileName;

                _context.Add(productGroup);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(productGroup);
        }

        // GET: Admin/ProductGroups/Edit/5
        [Title("ویرایش")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ProductGroups == null)
            {
                return NotFound();
            }

            var productGroup = await _context.ProductGroups.FindAsync(id);
            if (productGroup == null)
            {
                return NotFound();
            }
            return View(productGroup);
        }

        // POST: Admin/ProductGroups/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Title("ویرایش")]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("ProductGroupID,ProductGroupTitle,ProductGroupImageFilename")] ProductGroup productGroup
            , IFormFile? ProductGroupImageFilenameUpload)
        {
            if (id != productGroup.ProductGroupID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (ProductGroupImageFilenameUpload != null)
                    {
                        string FileName = Guid.NewGuid().ToString().Replace("-", "")
                                          + Path.GetExtension(ProductGroupImageFilenameUpload.FileName);
                        string imagesPath = Path.Combine(Environment.WebRootPath, "Uploads", "Images", imageSavePath);
                        string SavePath = Path.Combine(imagesPath, FileName);
                        string oldSavePath = Path.Combine(imagesPath, productGroup.ProductGroupImageFilename);

                        using (var stream = System.IO.File.Create(SavePath))
                        {
                            ProductGroupImageFilenameUpload.CopyTo(stream);
                        }
                        System.IO.File.Delete(oldSavePath);
                        productGroup.ProductGroupImageFilename = FileName;
                    }
                    _context.Update(productGroup);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductGroupExists(productGroup.ProductGroupID))
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
            return View("Edit", productGroup);
        }

        // GET: Admin/ProductGroups/Delete/5
        [Title("حذف")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ProductGroups == null)
            {
                return NotFound();
            }

            var productGroup = await _context.ProductGroups
                .FirstOrDefaultAsync(m => m.ProductGroupID == id);
            if (productGroup == null)
            {
                return NotFound();
            }

            return View(productGroup);
        }

        // POST: Admin/ProductGroups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Title("حذف")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ProductGroups == null)
            {
                return Problem("Entity set 'ApplicationDbContext.ProductGroups'  is null.");
            }
            var productGroup = await _context.ProductGroups.FindAsync(id);
            if (productGroup != null)
            {
                string imagesPath = Path.Combine(Environment.WebRootPath, "Uploads", "Images", imageSavePath);
                string oldSavePath = Path.Combine(imagesPath, productGroup.ProductGroupImageFilename);
                System.IO.File.Delete(oldSavePath);
                _context.ProductGroups.Remove(productGroup);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductGroupExists(int id)
        {
            return (_context.ProductGroups?.Any(e => e.ProductGroupID == id)).GetValueOrDefault();
        }
    }
}
