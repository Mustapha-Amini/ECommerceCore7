using ECommerceCore7.Data;
using ECommerceCore7.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkiaSharp;
using System.Collections.Generic;

namespace ECommerceCore7.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ReportsController : Controller
    {
        private ApplicationDbContext db;

        public ReportsController(ApplicationDbContext context)
        {
            db = context;
        }

        public async Task<IActionResult> TopUsersWithPurchases()
        {
            var startDate = DateTime.Now;

            var orders1 = db.Orders
                .Include(o => o.User)
                .Include(o => o.OrderDetails).ThenInclude(od => od.Product)
                .Select(o => new
                {
                    o.OrderID,
                    o.UserID,
                    OrderTotal = o.OrderDetails.Select(od => od.ProductCount * od.Product.ProductPrice).Sum()
                })
                // Where Tarikh >= Folan && Tarikh <= Bisar
                .ToList();

            var orders2 =
                orders1.GroupBy(x => x.UserID)
                .Select(y => new
                {
                    UserID = y.Key,
                    TotalPurchases = y.Sum(z => (long)z.OrderTotal)
                })
                .OrderByDescending(o => o.TotalPurchases)
                .ToList();

            var userIDs = orders2.Select(o => o.UserID).ToList();
            var users = db.Users.Where(u => userIDs.Contains(u.Id)).ToList();

            var finalResult =
                (from o in orders2
                 join u in users
                     on o.UserID equals u.Id
                 select new TopUsersWithPurchasesViewModel()
                 {
                     TotalPurchases = o.TotalPurchases,
                     UserName = u.UserName,
                     Fullname = u.Fullname,
                     PhoneNumber = u.PhoneNumber,
                     UserID = u.Id
                 }).ToList();

            return View(finalResult);
        }
    }
}
