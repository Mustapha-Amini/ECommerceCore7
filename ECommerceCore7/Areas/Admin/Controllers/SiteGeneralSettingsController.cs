using ECommerceCore7.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NuGet.Configuration;
using System.Data;

namespace ECommerceCore7.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class SiteGeneralSettingsController : Controller
    {
        private SiteGeneralSettings Settings;
        private IWebHostEnvironment Environment;

        public SiteGeneralSettingsController(IWebHostEnvironment environment)
        {
            Environment = environment;
        }

        [Title("مدیریت تنظیمات سایت")]
        public IActionResult Index()
        {
            ReadSettings();
            return View(Settings);
        }

        [HttpPost]
        public IActionResult Index(SiteGeneralSettings model)
        {
            if (ModelState.IsValid)
            {
                Settings = model;
                WriteSettings();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        private void WriteSettings()
        {

            string SettingsFileName = Path.Combine(Environment.ContentRootPath, "SiteGeneralSettings.json");

            var jsonConverted = Newtonsoft.Json.JsonConvert
                .SerializeObject(Settings,
                    Formatting.Indented,
                    new JsonSerializerSettings()
                    {
                        ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                    }
                );
            System.IO.File.WriteAllText(SettingsFileName, jsonConverted);
        }

        private void ReadSettings()
        {
            string SettingsFileName = Path.Combine(Environment.ContentRootPath, "SiteGeneralSettings.json");

            if (System.IO.File.Exists(SettingsFileName))
            {
                var settingsFileContent = System.IO.File.ReadAllText(SettingsFileName);
                Settings = JsonConvert.DeserializeObject<SiteGeneralSettings>(settingsFileContent);
            }
            else
            {
                Settings = new SiteGeneralSettings();
            }
        }
    }
}
