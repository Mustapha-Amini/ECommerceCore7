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
    [ControllerTitle("تصاویر کالاها", "تصویر کالا")]
    public class ProductImagesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private IWebHostEnvironment Environment;
        private string imageSavePath = "Products";

        public ProductImagesController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            Environment = environment;
        }

        // GET: Admin/ProductImages
        [Title("فهرست")]
        public async Task<IActionResult> Index(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Products");
            }

            var product = _context.Products.Find(id);
            ViewBag.ProductTitle = product.ProductTitle;
            ViewBag.ProductID = product.ProductID;
            var productImage = _context.ProductImages.Include(p => p.Product)
                .Where(pi => pi.ProductID == id.Value);
            return View(await productImage.ToListAsync());
        }

        // GET: Admin/ProductImages/Details/5


        // GET: Admin/ProductImages/Create

        [Title("افزودن")]
        public IActionResult Create(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Products");
            }

            //ViewData["ProductID"] = new SelectList(_context.Products, "ProductID", "ProductLongDescription");
            var model = new ProductImage() { ProductID = id.Value };
            return View(model);
        }

        // POST: Admin/ProductImages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Title("افزودن")]
        public async Task<IActionResult> Create(
            [Bind("ProductImageID,ProductID,ProductImageFilename")] ProductImage productImage,
            IFormFile? ProductImageFilenameUpload)
        {
            if (ModelState.IsValid)
            {
                string imagesPath = Path.Combine(Environment.WebRootPath, "Uploads", "Images", imageSavePath);
                string FileName = Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(ProductImageFilenameUpload.FileName);
                string SavePath = Path.Combine(imagesPath, FileName);
                using (var stream = System.IO.File.Create(SavePath))
                {
                    ProductImageFilenameUpload.CopyTo(stream);
                }
                productImage.ProductImageFilename = FileName;


                _context.Add(productImage);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { id = productImage.ProductID });
            }
            //ViewData["ProductID"] = new SelectList(_context.Products, "ProductID", "ProductLongDescription", productImage.ProductID);
            return View(productImage);
        }



        // GET: Admin/ProductImages/Delete/5
        [Title("حذف")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ProductImages == null)
            {
                return NotFound();
            }

            var productImage = await _context.ProductImages
                .Include(p => p.Product)
                .FirstOrDefaultAsync(m => m.ProductImageID == id);
            if (productImage == null)
            {
                return NotFound();
            }

            return View(productImage);
        }

        // POST: Admin/ProductImages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Title("حذف")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ProductImages == null)
            {
                return Problem("Entity set 'ApplicationDbContext.ProductImages'  is null.");
            }
            var productImage = await _context.ProductImages.FindAsync(id);

            if (productImage != null)
            {
                string imagesPath = Path.Combine(Environment.WebRootPath, "Uploads", "Images", imageSavePath);
                string oldSavePath = Path.Combine(imagesPath, productImage.ProductImageFilename);
                System.IO.File.Delete(oldSavePath);

                _context.ProductImages.Remove(productImage);
            }
            int productID = productImage.ProductID;


            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { id = productID });
        }

        private bool ProductImageExists(int id)
        {
            return (_context.ProductImages?.Any(e => e.ProductImageID == id)).GetValueOrDefault();
        }
    }
}
