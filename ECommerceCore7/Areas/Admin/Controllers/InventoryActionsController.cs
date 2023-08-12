using ECommerceCore7.Data;
using ECommerceCore7.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ECommerceCore7.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    [ControllerTitle("", "")]
    public class InventoryActionsController : Controller
    {
        private readonly ApplicationDbContext db;

        public InventoryActionsController(ApplicationDbContext context)
        {
            db = context;
        }

        // GET: Admin/InventoryActions
        [Title("فهرست")]
        [Icon("fa-duotone fa-file-lines")]
        public async Task<IActionResult> Index()
        {
            var model = await (db.InventoryActions.Include(i => i.InventoryActionType).Include(i => i.Order).Include(i => i.Product).Include(i => i.User)).ToListAsync();
            return View(model);
        }


        [Title("جزئیات")]
        [Icon("fa-duotone fa-file-circle-info")]
        // GET: Admin/InventoryActions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || db.InventoryActions == null)
            {
                return NotFound();
            }

            var inventoryAction = await db.InventoryActions
                .Include(i => i.InventoryActionType)
                .Include(i => i.Order)
                .Include(i => i.Product)
                .Include(i => i.User)
                .FirstOrDefaultAsync(m => m.InventoryActionID == id);
            if (inventoryAction == null)
            {
                return NotFound();
            }

            return View(inventoryAction);
        }

        // GET: Admin/InventoryActions/Create
        [Title("ایجاد")]
        [Icon("fa-duotone fa-file-circle-info")]
        public IActionResult Create()
        {
            //ViewData["InventoryActionTypeID"] = new SelectList(db.InventoryActionTypes, "InventoryActionID", "InventoryActionTypeTitle");
            //ViewData["OrderID"] = new SelectList(db.Orders, "OrderID", "OrderID");
            //ViewData["UserID"] = new SelectList(db.Users, "Id", "UserName");

            ViewData["ProductID"] = new SelectList(db.Products, "ProductID", "ProductTitle");

            var model = new InventoryAction() { InventoryActionDate = DateTime.Now };

            return View(model);
        }

        // POST: Admin/InventoryActions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Title("ایجاد")]
        [Icon("fa-duotone fa-file-circle-info")]
        public async Task<IActionResult> Create(
            [Bind("InventoryActionID,InventoryActionTypeID,ProductID,UserID,OrderID,InventoryActionComments,InventoryActionDate,InventoryActionCount")]
            InventoryAction inventoryAction)
        {
            // Get Logged In User ID
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var UserID = Convert.ToInt32(userIdString);

            inventoryAction.UserID = UserID;
            inventoryAction.InventoryActionTypeID = 1;


            if (ModelState.IsValid)
            {
                db.Add(inventoryAction);
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            //ViewData["InventoryActionTypeID"] = new SelectList(db.InventoryActionTypes, "InventoryActionID", "InventoryActionTypeTitle", inventoryAction.InventoryActionTypeID);
            //ViewData["OrderID"] = new SelectList(db.Orders, "OrderID", "OrderID", inventoryAction.OrderID);
            //ViewData["UserID"] = new SelectList(db.Users, "Id", "UserName", inventoryAction.UserID);


            ViewData["ProductID"] = new SelectList(db.Products, "ProductID", "ProductLongDescription", inventoryAction.ProductID);

            return View(inventoryAction);
        }
    }
}
