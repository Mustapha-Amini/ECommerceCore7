using CheckBoxList.Core.Models;
using ECommerceCore7.Areas.Admin.Models;
using ECommerceCore7.Data;
using ECommerceCore7.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NuGet.Packaging;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceCore7.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    [ControllerTitle("کاربران", "کاربر")]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ApplicationDbContext context;
        private readonly ILogger<CreateUserModel> logger;

        public UsersController(
            UserManager<ApplicationUser> _userManager,
            ApplicationDbContext _context,
            ILogger<CreateUserModel> _logger)
        {
            logger = _logger;
            context = _context;
            this.userManager = _userManager;
        }
        [Title("فهرست")]
        public async Task<IActionResult> Index()
        {
            var model = await userManager.Users.ToListAsync();
            return View(model);
        }

        [Title("افزودن")]
        public async Task<IActionResult> Create()
        {
            var roles = context.Roles.ToList();
            var rolesList = roles.Select(r => new CheckBoxItem(r.Id, r.Name, false, false)).ToList();
            ViewBag.RolesList = rolesList;
            return View();
        }

        [HttpPost]
        [Title("افزودن")]
        public async Task<IActionResult> Create(CreateUserModel model, List<int> selectedRoles)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Username, Email = model.Email };
                var result = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    if (selectedRoles.Any())
                    {
                        var roles =
                            context.Roles.Where(r => selectedRoles.Contains(r.Id))
                            .Select(r => r.Name).Distinct().ToList();

                        /*
                        foreach (var role in roles)
                        {
                            await userManager.AddToRoleAsync(user, role);
                        }
                        */
                        var userRoles = selectedRoles.Select(sr => new ApplicationUserRole()
                        {
                            UserId = user.Id,
                            RoleId = sr
                        }).ToList();
                        await context.UserRoles.AddRangeAsync(userRoles);
                        await context.SaveChangesAsync();
                    }

                    logger.LogInformation("User created a new account with password.");
                    return RedirectToAction("Index");
                }
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                var _roles = context.Roles.ToList();
                var rolesList = _roles.Select(r => new CheckBoxItem(
                    r.Id,
                    r.Name,
                    selectedRoles.Contains(r.Id),
                    false)).ToList();
                ViewBag.RolesList = rolesList;
            }
            return View(model);
        }

        [Title("ویرایش")]
        public async Task<IActionResult> Edit(int? id)
        {
            // Load Current User's Roles List
            var userRoles =
                context.UserRoles.Where(ur => ur.UserId == id).ToList();

            var roles = context.Roles.ToList();
            var rolesList =
                roles.Select(r => new CheckBoxItem(
                    r.Id,
                    r.Name,
                    userRoles.Any(ur => ur.RoleId == r.Id),
                    false)).ToList();
            ViewBag.RolesList = rolesList;

            // Load the user's itself
            var user = await context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) return NotFound($"کاربری با شناسه {id} یافت نشد");

            var model = new EditUserModel()
            {
                UserID = user.Id,
                Username = user.UserName
            };

            return View(model);
        }

        [HttpPost]
        [Title("ویرایش")]
        public async Task<IActionResult> Edit(EditUserModel model, List<int> selectedRoles)
        {
            if (ModelState.IsValid)
            {
                var user =
                    await context.Users.Include(u => u.UserRoles).FirstOrDefaultAsync(u => u.Id == model.UserID);
                if (user != null)
                {
                    var currentUserRoles = user.UserRoles.Select(ur => ur.RoleId).ToList();
                    if (!currentUserRoles.HaveSameElements(selectedRoles))
                    {
                        user.UserRoles.Clear();
                        var newRolesList = selectedRoles.Select(sr => new ApplicationUserRole()
                        {
                            RoleId = sr,
                            UserId = user.Id
                        }).ToList();
                        user.UserRoles.AddRange(newRolesList);
                    }

                    user.UserName = model.Username;
                    await context.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                else
                {
                    return NotFound($"کاربری با شناسه {model.UserID} یافت نشد");
                }
            }
            var roles = await context.Roles.ToListAsync();
            var rolesList =
                roles.Select(r => new CheckBoxItem(
                    r.Id,
                    r.Name,
                    selectedRoles.Any(sr => sr == r.Id),
                    false)).ToList();
            ViewBag.RolesList = rolesList;
            return View(model);
        }

        public async Task<IActionResult> Delete()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(ApplicationUser model)
        {
            return View();
        }

        [Title("تغییر کلمه عبور")]
        public async Task<IActionResult> ChangePassword(int? id)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) return NotFound($"کاربری با شناسه {id} یافت نشد");
            var model = new ChangePasswordByAdminModel()
            {
                UserID = user.Id,
                Username = user.UserName
            };

            return View(model);
        }

        [HttpPost]
        [Title("تغییر کلمه عبور")]
        public async Task<IActionResult> ChangePassword(ChangePasswordByAdminModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await context.Users.FirstOrDefaultAsync(u => u.Id == model.UserID);
                var token = await userManager.GeneratePasswordResetTokenAsync(user);
                await userManager.ResetPasswordAsync(user, token, model.Password);
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }
    }
}
