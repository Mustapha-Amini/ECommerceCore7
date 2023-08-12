using ECommerceCore7.Data;
using ECommerceCore7.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerceCore7.Areas.Admin
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    [ControllerTitle("گروه های کاربران", "گروه کاربر")]
    public class RolesController : Controller
    {

        private readonly RoleManager<ApplicationRole> roleManager;
        private readonly ApplicationDbContext context;

        public RolesController(
            RoleManager<ApplicationRole> _roleManager,
            ApplicationDbContext _context)
        {
            context = _context;
            this.roleManager = _roleManager;
        }

        [Title("فهرست")]
        public async Task<IActionResult> Index()
        {
            var model = roleManager.Roles.ToList();
            return View(model);
        }

        [Title("افزودن")]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        [Title("افزودن")]
        public async Task<IActionResult> Create(ApplicationRole model)
        {
            model.ConcurrencyStamp = Guid.NewGuid().ToString().ToLower();
            if (ModelState.IsValid)
            {
                //await _roleManager.CreateAsync(model);
                await context.Roles.AddAsync(model);
                await context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View();
        }

        [Title("ویرایش")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var model = context.Roles.FirstOrDefault(r => r.Id == id);
            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }

        [HttpPost]
        [Title("ویرایش")]
        public async Task<IActionResult> Edit(int id, ApplicationRole role)
        {
            if (id != role.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    context.Update(role);
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {

                }
                return RedirectToAction(nameof(Index));
            }
            return View(role);
        }
    }
}
