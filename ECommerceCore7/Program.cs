using ECommerceCore7.Classes.Identity;
using ECommerceCore7.Classes.Routing;
using ECommerceCore7.Data;
using ECommerceCore7.Models.Identity;
using ECommerceCore7.Services;
using ImageResizer.AspNetCore.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using SeoTags;
using System.Globalization;

namespace ECommerceCore7
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CultureInfo.DefaultThreadCurrentCulture
                = CultureInfo.DefaultThreadCurrentUICulture = PersianDateExtensionMethods.GetPersianCulture();

            var builder = WebApplication.CreateBuilder(args);

            #region Configure ImageResizer: Part 1
            //Add ImageResizer - Step 1:
            builder.Services.AddSingleton<IFileProvider>(_ =>
                new PhysicalFileProvider(builder.Environment.WebRootPath ?? builder.Environment.ContentRootPath));
            //Add ImageResizer - Step 2:
            builder.Services.AddImageResizer();
            #endregion


            #region Read Connection String and Configure DbContext
            var connectionString =
                builder.Configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString)
                , ServiceLifetime.Transient);
            #endregion


            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            #region Identity Customization
            builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
                {
                    options.SignIn.RequireConfirmedAccount = false;
                    options.User.RequireUniqueEmail = false;
                })
                //.AddRoles<ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders()
                .AddErrorDescriber<PersianIdentityErrorDescriber>();
            #endregion

            #region TicketStore Configuration
            // Add HttpContextAccessor for further use.
            //builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            // Add ITicketStore Implementation which needs dbContext:
            //builder.Services.AddSingleton<ITicketStore, DatabaseTicketStore>();
            // Register this custom ticket store as CookieAuthenticationOptions:
            //builder.Services.AddOptions<CookieAuthenticationOptions>(IdentityConstants.ApplicationScheme)
            //    .Configure<ITicketStore>((options, store) => { options.SessionStore = store; });
            #endregion

            #region Data Protection Configuration
            // After each publish/deploy , all logged in users are logged out automatically.
            // To Fix that, following lines (AddDataProtection) added:
            // https://stackoverflow.com/questions/40575776/asp-net-core-identity-login-status-lost-after-deploy/49563627#49563627
            string dataProtectionStorePath = Path.Combine(builder.Environment.ContentRootPath, "DataProtection", "Keys");
            if (!Directory.Exists(dataProtectionStorePath))
            {
                Directory.CreateDirectory(dataProtectionStorePath);
            }
            builder.Services.AddDataProtection()
                // This helps surviving a restart: a same app will find back its keys. Just ensure to create the folder.
                .PersistKeysToFileSystem(new DirectoryInfo(dataProtectionStorePath))
                // This helps surviving a site update: each app has its own store, building the site creates a new app
                .SetApplicationName("ECommerceCore7")
                .SetDefaultKeyLifetime(TimeSpan.FromDays(90));
            #endregion

            builder.Services.AddSeoTags(seoInfo =>
            {
                seoInfo.SetSiteInfo(
                    siteTitle: SiteGeneralSettings.GetSettings().SiteName,
                    //siteTwitterId: "@MySiteTwitter",  //optional
                    //siteFacebookId: "https://facebook.com/MySite",  //optional
                    //openSearchUrl: "https://site.com/open-search.xml",  //optional
                    robots: "index, follow"  //optional
                );

                //optional
                seoInfo.SetLocales("fa_IR");
            });

            #region Configure Authentication Cookie and Custom User Store
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.SlidingExpiration = true;
                options.ExpireTimeSpan = System.TimeSpan.FromDays(30);
                //options.Cookie.IsEssential = true;

                options.LoginPath = new PathString("/Identity/Account/Login");
                options.LogoutPath = new PathString("/Identity/Account/Logout");
                options.AccessDeniedPath = new PathString("/Identity/Account/AccessDenied");
            });
            builder.Services.AddTransient<IUserStore<ApplicationUser>, UserStore>();
            #endregion


            #region Configure Sms and Email Service
            builder.Services.AddTransient<IEmailSender, EmailSender>();
            builder.Services.AddScoped<ISmsService, SmsService>();
            #endregion

            //Comment

            #region Add Global Filters
            builder.Services.AddControllersWithViews(options =>
            {
                // Enable Icon and Title Filter
                options.Filters.Add(typeof(IconAndTitleActionFilter));
            });
            #endregion

            var app = builder.Build();

            #region Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            #endregion

            #region Error Handling
            //app.UseStatusCodePages();
            app.UseStatusCodePagesWithReExecute("/Home/HandleError/{0}");
            #endregion

            app.UseHttpsRedirection();

            #region Configure ImageResizer: Part 2
            //Add ImageResizer - Step 3:
            app.UseImageResizer();
            #endregion

            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "staticpages",
                pattern: "{*permalink}",
                defaults: new
                {
                    controller = "Home",
                    action = "ShowStaticPage"
                },
                constraints: new
                {
                    //permalink = new StaticPagesUrlConstraintDb()
                    permalink = new StaticPagesUrlConstraintFileCache()
                });

            app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
            );

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}