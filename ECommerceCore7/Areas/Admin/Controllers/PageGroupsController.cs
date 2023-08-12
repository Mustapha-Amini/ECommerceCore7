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
    [ControllerTitle("گروه های صفحات", "گروه صفحه")]

    public class PageGroupsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PageGroupsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/PageGroups
        [Title("فهرست")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.PageGroups.ToListAsync());
        }

        // GET: Admin/PageGroups/Details/5
        [Title("جزئیات")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.PageGroups == null)
            {
                return NotFound();
            }

            var pageGroup = await _context.PageGroups
                .FirstOrDefaultAsync(m => m.PageGroupID == id);
            if (pageGroup == null)
            {
                return NotFound();
            }

            return View(pageGroup);
        }

        // GET: Admin/PageGroups/Create
        [Title("افزودن")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/PageGroups/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Title("افزودن")]
        public async Task<IActionResult> Create([Bind("PageGroupID,PageGroupTitle,PageGroupIcon")] PageGroup pageGroup)
        {
            if (ModelState.IsValid)
            {
                _context.Add(pageGroup);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(pageGroup);
        }

        // GET: Admin/PageGroups/Edit/5
        [Title("ویرایش")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.PageGroups == null)
            {
                return NotFound();
            }

            var pageGroup = await _context.PageGroups.FindAsync(id);
            if (pageGroup == null)
            {
                return NotFound();
            }
            return View(pageGroup);
        }

        // POST: Admin/PageGroups/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Title("ویرایش")]
        public async Task<IActionResult> Edit(int id, [Bind("PageGroupID,PageGroupTitle,PageGroupIcon")] PageGroup pageGroup)
        {
            if (id != pageGroup.PageGroupID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pageGroup);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PageGroupExists(pageGroup.PageGroupID))
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
            return View(pageGroup);
        }

        // GET: Admin/PageGroups/Delete/5
        [Title("حذف")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.PageGroups == null)
            {
                return NotFound();
            }

            var pageGroup = await _context.PageGroups
                .FirstOrDefaultAsync(m => m.PageGroupID == id);
            if (pageGroup == null)
            {
                return NotFound();
            }

            return View(pageGroup);
        }

        // POST: Admin/PageGroups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Title("حذف")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.PageGroups == null)
            {
                return Problem("Entity set 'ApplicationDbContext.PageGroups'  is null.");
            }
            var pageGroup = await _context.PageGroups.FindAsync(id);
            if (pageGroup != null)
            {
                _context.PageGroups.Remove(pageGroup);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PageGroupExists(int id)
        {
            return _context.PageGroups.Any(e => e.PageGroupID == id);
        }
    }
}
