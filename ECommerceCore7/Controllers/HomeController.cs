using Dto.Payment;
using ECommerceCore7.Data;
using ECommerceCore7.Models;
using ECommerceCore7.Models.Entities;
using ECommerceCore7.Models.Identity;
using ECommerceCore7.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SkiaSharp;
using System.Collections.Immutable;
using System.Configuration;
using System.Diagnostics;
using System.Security.Claims;
using ZarinPal.Class;

namespace ECommerceCore7.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private ApplicationDbContext db;

        private readonly Payment _payment;
        private readonly Authority _authority;
        private readonly Transactions _transactions;

        private readonly UserManager<ApplicationUser> _userManager;
        private IConfiguration configuration { get; }

        public HomeController(
            ILogger<HomeController> logger,
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IConfiguration Configuration)
        {
            _userManager = userManager;
            _logger = logger;
            db = context;

            var expose = new Expose();
            _payment = expose.CreatePayment();
            _authority = expose.CreateAuthority();
            _transactions = expose.CreateTransactions();

            configuration = Configuration;
        }

        public IActionResult Index()
        {
            PostCostViewModel pc = new PostCostViewModel()
            {
                PackageHeight = 100,
                PackageLength = 120,
                PackageWidth = 90,
                PackageWeight = 4000
            };
            var postIrRequest = JsonConvert.SerializeObject(pc);

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Route("/Home/HandleError/{code:int}")]
        public IActionResult HandleError(int code)
        {
            //ViewData["ErrorMessage"] = $"Error occurred. The ErrorCode is: {code}";
            if (code == 404)
            {
                return View("Message", new MessageViewModel()
                {
                    MainTitle = "خطای 404",
                    MessageType = MessageType.Danger,
                    SubTitle = "صفحه درخواستی شما در سایت وجود ندارد یا غیرفعال شده است.",
                    UrlToGoTitle = "بازگشت به صفحه اصلی",
                    UrlToGo = Url.Action("Index", "Home", new { Area = "" })
                });
            }
            else
            {
                return View("Message", new MessageViewModel()
                {
                    MainTitle = "خطای نامشخص",
                    MessageType = MessageType.Danger,
                    SubTitle = $"کد خطا: {code}",
                    UrlToGoTitle = "بازگشت به صفحه اصلی",
                    UrlToGo = Url.Action("Index", "Home", new { Area = "" })
                });
            }
        }

        #region My Additional Actions
        public IActionResult ProductsByGroup(int? id)
        {
            var products =
                db.Products
                    .Include(p => p.ProductImages)
                    .Include(p => p.ProductGroup)
                    .Where(p => p.ProductGroupID == id.Value && p.ProductIsActive).ToList();
            var groupTitle = products.FirstOrDefault()?.ProductGroup?.ProductGroupTitle;
            ViewBag.GroupTitle = groupTitle;
            return View(products);
        }

        public IActionResult Product(int? id)
        {
            var product =
                db.Products
                    .Include(p => p.ProductImages)
                    .Include(p => p.ProductTags).ThenInclude(pt => pt.Tag)
                    .FirstOrDefault(p => p.ProductID == id.Value);

            if (product == null || !product.ProductIsActive)
            {
                return NotFound();
            }

            return View(product);
        }

        public IActionResult AddToShoppingCart(int? ProductID, int? ProductCount)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Forbid();
            }
            // Get Logged In User ID
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var UserID = Convert.ToInt32(userIdString);

            // Check If User Has Any Order Which Is Not Finalized:
            var userOrder =
                db.Orders.Include(o => o.OrderDetails)
                .FirstOrDefault(o => o.UserID == UserID && o.IsFinalized == false);

            if (userOrder == null)
            {
                userOrder = new Order()
                {
                    IsFinalized = false,
                    UserID = UserID,
                    OrderDate = DateTime.Now,
                    OrderDetails = new List<OrderDetail>()
                };
                db.Orders.Add(userOrder);
            }

            // Check if userOrder Already Contains A Product with same ProductID
            var orderDetail = userOrder?.OrderDetails?.FirstOrDefault(od => od.ProductID == ProductID);

            if (orderDetail == null)
            {
                userOrder.OrderDetails.Add(new OrderDetail()
                {
                    ProductID = ProductID.Value,
                    ProductCount = ProductCount.Value
                });
            }
            else
            {
                orderDetail.ProductCount += ProductCount.Value;
            }

            db.SaveChanges();

            var totalCartItems = userOrder.OrderDetails.Select(od => od.ProductCount).Sum();
            return Json(totalCartItems);
        }

        public IActionResult GetShoppingCartItemsCount()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Forbid();
            }

            // Get Logged In User ID
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var UserID = Convert.ToInt32(userIdString);

            // Check If User Has Any Order Which Is Not Finalized:
            var userOrder =
                db.Orders.Include(o => o.OrderDetails)
                    .FirstOrDefault(o => o.UserID == UserID && o.IsFinalized == false);

            var totalCount = userOrder?.OrderDetails.Select(od => od.ProductCount).Sum() ?? 0;
            return Json(totalCount);
        }

        [Authorize]
        public IActionResult Cart()
        {
            // Get Logged In User ID
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var UserID = Convert.ToInt32(userIdString);

            // Check If User Has Any Order Which Is Not Finalized:
            var userOrder =
                db.Orders
                    .Include(o => o.OrderDetails).ThenInclude(od => od.Product)
                    //.ThenInclude(p => p.ProductImages)
                    .FirstOrDefault(o => o.UserID == UserID && o.IsFinalized == false);
            if (userOrder == null)
            {
                return RedirectToAction("Orders");
            }
            return View(userOrder);
        }

        [Authorize]
        public IActionResult RemoveProductFromCart(int? id)
        {
            // Get Logged In User ID
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var UserID = Convert.ToInt32(userIdString);

            // Check If User Has Any Order Which Is Not Finalized:
            var userOrder =
                db.Orders
                    .Include(o => o.OrderDetails).ThenInclude(od => od.Product)
                    //.ThenInclude(p => p.ProductImages)
                    .FirstOrDefault(o => o.UserID == UserID && o.IsFinalized == false);

            var orderDetail = userOrder.OrderDetails.FirstOrDefault(od => od.ProductID == id);
            userOrder.OrderDetails.Remove(orderDetail);
            db.SaveChanges();

            // Delete Product From Cart
            return RedirectToAction("Cart");
        }


        public IActionResult PagesByGroup(int? id)
        {
            var pageGroup = db.PageGroups.FirstOrDefault(pg => pg.PageGroupID == id.Value);
            ViewBag.PageGroupTitle = pageGroup.PageGroupTitle;
            ViewBag.PageGroupIcon = pageGroup.PageGroupIcon;

            var pages =
                db.Pages
                    .Include(p => p.User)
                    .Where(p => p.PageGroupID == id.Value && string.IsNullOrEmpty(p.PageRoute)).ToList();

            return View(pages);
        }

        public IActionResult Page(int? id)
        {
            var page =
                db.Pages
                    .Include(p => p.PageGroup)
                    .Include(p => p.User)
                    .Include(p => p.PageTags).ThenInclude(pt => pt.Tag)
                    .FirstOrDefault(p => p.PageID == id.Value);

            return View(page);
        }

        public IActionResult AddToWishlist(int? ProductID)
        {
            var actionResult = false;

            if (!User.Identity.IsAuthenticated)
            {
                return Forbid();
            }
            // Get Logged In User ID
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var UserID = Convert.ToInt32(userIdString);

            var currentWishlist =
                db.Wishlists.Where(wl => wl.UserID == UserID).ToList();

            if (!currentWishlist.Any(wl => wl.ProductID == ProductID))
            {
                db.Wishlists.Add(new Wishlist()
                {
                    UserID = UserID,
                    ProductID = ProductID.Value,
                    AddedToListDate = DateTime.Now
                });
                db.SaveChanges();
                actionResult = true;
            }

            return Json(actionResult);
        }

        public IActionResult Wishlist()
        {
            // Get Logged In User ID
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var UserID = Convert.ToInt32(userIdString);

            // Check If User Has Any Order Which Is Not Finalized:
            var userWishlist =
                db.Wishlists
                    .Include(o => o.Product)
                    .Where(w => w.UserID == UserID)
                    .OrderByDescending(w => w.AddedToListDate).ToList();

            return View(userWishlist);
        }

        public IActionResult GetWishlistItemCount()
        {
            // Get Logged In User ID
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var UserID = Convert.ToInt32(userIdString);

            var wishListItemCount = db.Wishlists.Count(wl => wl.UserID == UserID);
            return Json(wishListItemCount);
        }

        public IActionResult RemoveProductFromWishlist(int? ProductID)
        {
            // Get Logged In User ID
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var UserID = Convert.ToInt32(userIdString);

            // Check If User Has Any Order Which Is Not Finalized:
            var userWishlist =
                db.Wishlists.FirstOrDefault(w => w.UserID == UserID && w.ProductID == ProductID.Value);
            db.Wishlists.Remove(userWishlist);
            db.SaveChanges();

            return RedirectToAction("Wishlist");
        }

        #endregion

        public async Task<IActionResult> ShowStaticPage()
        {
            var page = (Page)HttpContext.Items["CurrentStaticPage"];
            return View(page);
        }


        [Authorize]
        public async Task<IActionResult> ProcessOnlinePayment(int id) // id is OrderID in this action
        {
            var order = await db.Orders
                .Include(o => o.User)
                .Include(o => o.OrderDetails).ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(m => m.OrderID == id);

            var orderTotal =
                order.OrderDetails.Select(od => od.ProductCount * od.Product.ProductPrice).Sum();

            string callbackUrl =
                Url.AbsoluteAction("ProcessOnlinePaymentResponse", "Home", new { id = id });

            var user = await _userManager.GetUserAsync(HttpContext.User);

            string ZarinpalMerchantID = SiteGeneralSettings.GetSettings().ZarinpalMerchantID;
            bool ZarinpalSandBoxIsActive = SiteGeneralSettings.GetSettings().ZarinpalSandBoxIsActive;

            Payment.Mode sandBoxMode = ZarinpalSandBoxIsActive ? Payment.Mode.sandbox : Payment.Mode.zarinpal;
            string ZarinPalUrlPrefix = ZarinpalSandBoxIsActive ? "sandbox." : "";

            try
            {
                var result = await _payment.Request(new DtoRequest()
                {
                    Mobile = user.PhoneNumber,
                    Email = user.Email,
                    CallbackUrl = callbackUrl,
                    Description = "توضیحات",
                    Amount = orderTotal,
                    MerchantId = ZarinpalMerchantID
                }, sandBoxMode);

                return Redirect($"https://{ZarinPalUrlPrefix}zarinpal.com/pg/StartPay/{result.Authority}");
            }
            catch (Exception ex)
            {
                return View("Message", new MessageViewModel()
                {
                    //MainTitle = "پرداخت موفق",
                    SubTitle = "خطا در ارتباط با زرین پال",
                    UrlToGoTitle = "دیدن سبد خرید",
                    MessageType = MessageType.Danger,
                    UrlToGo = Url.AbsoluteAction("Cart", "Home")
                });
            }

            return Content("Unknown Error?!");
        }
        public async Task<IActionResult> ProcessOnlinePaymentResponse(int id)
        {
            string? authority = Request.Query["authority"];
            string? status = Request.Query["status"];

            string ZarinpalMerchantID = SiteGeneralSettings.GetSettings().ZarinpalMerchantID;
            bool ZarinpalSandBoxIsActive = SiteGeneralSettings.GetSettings().ZarinpalSandBoxIsActive;

            Payment.Mode sandBoxMode = ZarinpalSandBoxIsActive ? Payment.Mode.sandbox : Payment.Mode.zarinpal;

            var order = await db.Orders
                .Include(o => o.User)
                .Include(o => o.OrderDetails).ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(m => m.OrderID == id);

            var orderTotal =
                order.OrderDetails.Select(od => od.ProductCount * od.Product.ProductPrice).Sum();

            if (status == "OK")
            {
                var verification = await _payment.Verification(new DtoVerification
                {
                    Amount = orderTotal,
                    MerchantId = ZarinpalMerchantID,
                    Authority = authority
                }, sandBoxMode);
                if (verification != null && verification.Status == 100)
                {
                    // تبدیل وضعیت سفارش به نهایی:
                    order.IsFinalized = true;
                    // کسر موجودی انبار بر حسب این فاکتور:

                    /*
                    foreach (var orderDetail in order.OrderDetails)
                    {
                        db.InventoryActions.Add(new InventoryAction()
                        {
                            InventoryActionDate = DateTime.Now,
                            OrderID = orderDetail.OrderID,
                            UserID = order.UserID,
                            ProductID = orderDetail.ProductID,
                            InventoryActionCount = orderDetail.ProductCount,
                            InventoryActionTypeID = 2,
                            InventoryActionComments = $"خروج کالا به ازای فاکتور شماره {order.OrderID}"
                        });
                    }
                    */
                    var inventoryActions = order.OrderDetails.Select(od => new InventoryAction()
                    {
                        InventoryActionDate = DateTime.Now,
                        OrderID = od.OrderID,
                        UserID = order.UserID,
                        ProductID = od.ProductID,
                        InventoryActionCount = od.ProductCount,
                        InventoryActionTypeID = 2,
                        InventoryActionComments = $"خروج کالا به ازای فاکتور شماره {order.OrderID}"
                    }).ToList();
                    await db.InventoryActions.AddRangeAsync(inventoryActions);


                    await db.SaveChangesAsync();


                    return View("Message", new MessageViewModel()
                    {
                        MessageType = MessageType.Success,
                        MainTitle = "پرداخت موفق",
                        SubTitle = $"با تشکر از پرداخت آنلاین شما. کد رهگیری: {verification.RefId}",
                        UrlToGoTitle = "دیدن فاکتور خرید",
                        UrlToGo = Url.AbsoluteAction("Orders", "Home")
                    });
                }
            }
            return View("Message", new MessageViewModel()
            {
                MessageType = MessageType.Danger,
                MainTitle = "پرداخت ناموفق",
                SubTitle = "خطایی در پرداخت آنلاین شما وجود داشت",
                UrlToGoTitle = "مشاهده سبد خرید",
                UrlToGo = Url.AbsoluteAction("Cart", "Home")
            });
        }

        public async Task<IActionResult> Orders()
        {
            // Get Logged In User ID
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var UserID = Convert.ToInt32(userIdString);

            var orders =
                db.Orders.Include(o => o.User)
                    .Where(o => o.UserID == UserID)
                    .OrderByDescending(o => o.OrderDate)
                    .ToList();
            return View(orders);
        }

        public async Task<IActionResult> OrderDetails(int? id)
        {
            if (id == null || db.Orders == null)
            {
                return NotFound();
            }

            var model = await db.Orders
                .Include(o => o.User)
                .Include(o => o.OrderDetails).ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(m => m.OrderID == id);
            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }
    }
}