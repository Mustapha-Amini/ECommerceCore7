using Newtonsoft.Json;
using NuGet.Configuration;
using System.ComponentModel.DataAnnotations;

namespace Microsoft.AspNetCore.Mvc
{
    public class SiteGeneralSettings
    {
        [Display(Name = "مسیر فایل لوگو")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string LogoFilePath { get; set; }

        [Display(Name = "نام نمایشی سایت")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string SiteName { get; set; }

        [Display(Name = "شعار سایت")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string SiteSlogan { get; set; }

        [Display(Name = "شماره تلفن پشتیبانی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string SupportPhoneNumber { get; set; }

        [Display(Name = "شناسه پذیرنده زرین پال")]
        public string ZarinpalMerchantID { get; set; }

        [Display(Name = "فعال بودن حالت تستی (SandBox) زرین پال")]
        public bool ZarinpalSandBoxIsActive { get; set; }

        [Display(Name = "توکن API پارس گرین")]
        public string ParsGreenApiToken { get; set; }

        [Display(Name = "شماره مبدا ارسال پیامک پارس گرین")]
        public string ParsGreenSendFromNumber { get; set; }

        [Display(Name = "تگ های متا صفحه اصلی")]
        public string HomePageMetaTags { get; set; }

        [Display(Name = "شرح متا صفحه اصلی")]
        public string HomePageDescription { get; set; }




        public static SiteGeneralSettings GetSettings()
        {
            HttpContext httpContext = new HttpContextAccessor().HttpContext;
            IWebHostEnvironment Environment =
                (IWebHostEnvironment)httpContext.RequestServices.GetService(typeof(IWebHostEnvironment));

            string SettingsFileName = Path.Combine(Environment.ContentRootPath, "SiteGeneralSettings.json");

            if (System.IO.File.Exists(SettingsFileName))
            {
                var settingsFileContent = System.IO.File.ReadAllText(SettingsFileName);
                var settings = JsonConvert.DeserializeObject<SiteGeneralSettings>(settingsFileContent);
                return settings;
            }
            else
            {
                return new SiteGeneralSettings();
            }
        }
    }
}
