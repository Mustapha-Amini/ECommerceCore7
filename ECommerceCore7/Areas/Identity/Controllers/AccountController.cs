﻿using ECommerceCore7.Areas.Identity.Models;
using ECommerceCore7.Data;
using ECommerceCore7.Models.Identity;
using ECommerceCore7.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;

namespace ECommerceCore7.Areas.Identity.Controllers
{
    [Area("Identity")]
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationDbContext db;
        private IConfiguration Configuration { get; }
        private readonly IEmailSender _emailSender;
        private readonly ILogger<RegisterModel> _logger;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ISmsService smsService;


        public AccountController(
            IConfiguration configuration,
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            ISmsService smsService
        )
        {
            db = context;
            Configuration = configuration;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            this.smsService = smsService;
        }

        public string ReturnUrl { get; set; }

        public async Task<IActionResult> AccessDenied()
        {
            return View();
        }

        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null) return RedirectToAction("Index", "Home");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound($"کاربری با شناسه '{userId}' یافت نشد.");

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ConfirmEmailAsync(user, code);
            ViewBag.StatusMessage =
                result.Succeeded ? "بابت تایید کردن ایمیل تان از شما متشکریم." : "خطا در تایید ایمیل شما.";
            return View();
        }

        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmailChange(string userId, string email, string code)
        {
            if (userId == null || email == null || code == null) return RedirectToAction("Index", "Home");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound($"خطا: کاربری با شناسه '{userId}' یافت نشد.");

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ChangeEmailAsync(user, email, code);
            if (!result.Succeeded)
            {
                ViewBag.StatusMessage = "خطا در بروزرسانی ایمیل.";
                return View();
            }

            // In our UI email and user name are one and the same, so when we update the email
            // we need to update the user name.
            /*
            var setUserNameResult = await _userManager.SetUserNameAsync(user, email);
            if (!setUserNameResult.Succeeded)
            {
                ViewBag.StatusMessage = "Error changing user name.";
                return View();
            }
            */
            await _signInManager.RefreshSignInAsync(user);
            ViewBag.StatusMessage = "بابت بروزرسانی ایمیل تان از شما متشکریم.";
            return View();
        }

        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null || !await _userManager.IsEmailConfirmedAsync(user))
                    // Don't reveal that the user does not exist or is not confirmed
                    return RedirectToAction("ForgotPasswordConfirmation");

                // For more information on how to enable account confirmation and password reset please 
                // visit https://go.microsoft.com/fwlink/?LinkID=532713
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Action(
                    "ResetPassword",
                    "Account",
                    new { area = "Identity", code },
                    Request.Scheme
                );

                await _emailSender.SendEmailAsync(
                    model.Email,
                    "بازنشانی کلمه عبور",
                    $"لطفا با کلیک کردن بر روی <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>این لینک</a> کلمه عبورتان را بازنشانی کنید.");

                return RedirectToAction("ForgotPasswordConfirmation");
            }

            return View();
        }

        [AllowAnonymous]
        public async Task<IActionResult> ForgotPasswordConfirmation()
        {
            return View();
        }

        [AllowAnonymous]
        public async Task<IActionResult> Lockout()
        {
            return View();
        }

        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            returnUrl ??= Url.Content("~/");
            ReturnUrl = returnUrl;
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model, string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/Identity/Manage");
            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(
                    model.Username,
                    model.Password,
                    model.RememberMe,
                    false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    return LocalRedirect(returnUrl);
                }

                if (result.RequiresTwoFactor)
                {
                    // ToDo: Implement LoginWith2fa using SMS
                    //return RedirectToAction("LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                }

                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToAction("Lockout");
                }

                ModelState.AddModelError(string.Empty, "نام کاربری یا کلمه عبور صحیح نیست.");
                return View();
            }

            return View();
        }

        [AllowAnonymous]
        public async Task<IActionResult> LoginWithSms()
        {
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> LoginWithSms(LoginWithSmsModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.PhoneNumber);
                if (user == null)
                {
                    ModelState.AddModelError("", "کاربری با این شماره موبایل در سیستم یافت نشد.");
                    return View();
                }

                // ToDo: Check if a non-expired code is already generated and Saved for this phone number.

                UserLoginWithSms loginWithSms = new UserLoginWithSms()
                {
                    PhoneNumber = user.PhoneNumber,
                    UserID = user.Id,
                    ExpireDate = DateTime.Now.AddMinutes(5)
                };
                loginWithSms.Initialize();
                await db.UserLoginWithSms.AddAsync(loginWithSms);
                await db.SaveChangesAsync();

                await smsService.SendSms(
                    $"کد امنیتی شما: {loginWithSms.AuthenticationCode}\r\n\r\nبرنامه نویس",
                    new List<string>() { loginWithSms.PhoneNumber }
                );
                return RedirectToAction("LoginWithSmsResponse", new { Key = loginWithSms.AuthenticationKey });
            }
            return View();
        }

        [AllowAnonymous]
        public async Task<IActionResult> LoginWithSmsResponse(string Key = null)
        {
            if (string.IsNullOrEmpty(Key))
            {
                return RedirectToAction("LoginWithSms");
            }
            var model = new LoginWithSmsResponseModel() { AuthenticationKey = Key };
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWithSmsResponse(LoginWithSmsResponseModel model)
        {
            if (ModelState.IsValid)
            {
                var responeRow =
                    db.UserLoginWithSms.Include(pr => pr.User)
                        .FirstOrDefault(pr => pr.AuthenticationKey == model.AuthenticationKey);
                if (responeRow == null)
                {
                    ModelState.AddModelError("", "کلید شناسایی معتبر نیست");
                    return View(model);
                }
                else
                {
                    if (model.AuthenticationCode != responeRow.AuthenticationCode)
                    {
                        ModelState.AddModelError("", "کد امنیتی وارد شده صحیح نیست");
                        return View(model);
                    }
                    else
                    {
                        await _signInManager.SignInAsync(responeRow.User, true);
                        _logger.LogInformation("User logged in.");
                        // return LocalRedirect(returnUrl);
                        return RedirectToAction("Index", "Manage", new { area = "Identity" });
                    }
                }
            }
            return View();
        }


        public async Task<IActionResult> Logout(string returnUrl = null)
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            if (returnUrl != null)
                return LocalRedirect(returnUrl);
            return RedirectToAction("Index", "Home", new { area = "" });
        }

        [AllowAnonymous]
        public async Task<IActionResult> PreRegister()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> PreRegister(UserPreRegisteration model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.PhoneNumber);
                if (user != null)
                {
                    ModelState.AddModelError("", $"شماره موبایل {model.PhoneNumber} توسط کاربر دیگری ثبت شده است.");
                    return View();
                }
                model.Initialize();
                model.ExpireTime = DateTime.Now.AddMinutes(5);
                await db.PreRegisterations.AddAsync(model);
                await db.SaveChangesAsync();
                await smsService.SendSms(
                    $"برنامه نویس:\r\n\r\nکد امنیتی شما: {model.AuthenticationCode}\r\n\r\nhttps://barnamenevis.net",
                    new List<string>() { model.PhoneNumber });
                return RedirectToAction("PreRegisterConfirm", new { Key = model.AuthenticationKey });
            }
            return View();
        }

        [AllowAnonymous]
        public IActionResult PreRegisterConfirm(string Key)
        {
            if (string.IsNullOrEmpty(Key))
            {
                return RedirectToAction("PreRegister");
            }
            var model = new UserPreRegisterationConfirm() { AuthenticationKey = Key };
            return View(model);
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult PreRegisterConfirm(UserPreRegisterationConfirm model)
        {
            if (ModelState.IsValid)
            {
                var confirmRow =
                    db.PreRegisterations.FirstOrDefault(pr => pr.AuthenticationKey == model.AuthenticationKey);
                if (confirmRow == null)
                {
                    ModelState.AddModelError("", "کلید شناسایی معتبر نیست");
                    return View(model);
                }
                else
                {
                    if (model.AuthenticationCode != confirmRow.AuthenticationCode)
                    {
                        ModelState.AddModelError("", "کد امنیتی وارد شده صحیح نیست");
                        return View();
                    }
                    else
                    {
                        confirmRow.Confirmed = true;
                        db.SaveChanges();
                        return RedirectToAction("Register", new { Key = model.AuthenticationKey });
                    }
                }
            }
            return View();
        }


        [AllowAnonymous]
        public IActionResult Register(string returnUrl = null, string Key = null)
        {
            bool IdentityPreRegisterationEnabled = Configuration.GetValue<bool>("Identity:PreRegisterationEnabled");
            if (IdentityPreRegisterationEnabled)
            {
                if (string.IsNullOrEmpty(Key))
                {
                    return RedirectToAction("PreRegister");
                }

                var confirmRow = db.PreRegisterations.FirstOrDefault(pr =>
                    pr.Confirmed == true && pr.AuthenticationKey == Key && pr.ExpireTime > DateTime.Now);
                if (confirmRow == null)
                {
                    return RedirectToAction("PreRegister");
                }
            }
            ReturnUrl = returnUrl;
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model, string returnUrl = null, string Key = null)
        {
            string phoneNumber = null;
            bool IdentityPreRegisterationEnabled = Configuration.GetValue<bool>("Identity:PreRegisterationEnabled");
            if (IdentityPreRegisterationEnabled)
            {
                if (string.IsNullOrEmpty(Key))
                {
                    return RedirectToAction("PreRegister");
                }

                var confirmRow = db.PreRegisterations.FirstOrDefault(pr =>
                    pr.Confirmed == true && pr.AuthenticationKey == Key && pr.ExpireTime > DateTime.Now);
                if (confirmRow == null)
                {
                    return RedirectToAction("PreRegister");
                }

                phoneNumber = confirmRow.PhoneNumber;
            }
            returnUrl ??= Url.Content("~/");
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Username, Email = model.Email };
                if (!string.IsNullOrEmpty(phoneNumber))
                {
                    user.PhoneNumber = phoneNumber;
                    user.PhoneNumberConfirmed = true;
                }

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Action(
                        "ConfirmEmail",
                        "Account",
                        new { area = "Identity", userId = user.Id, code, returnUrl },
                        Request.Scheme);

                    //await _emailSender.SendEmailAsync(model.Email, "Confirm your email",
                    //$"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                        return RedirectToAction("RegisterConfirmation",
                            new { email = model.Email, returnUrl });

                    await _signInManager.SignInAsync(user, false);
                    return LocalRedirect(returnUrl);
                }

                foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        [AllowAnonymous]
        public async Task<IActionResult> RegisterConfirmation(string email, string returnUrl = null)
        {
            var model = new RegisterConfirmationModel();

            if (email == null) return RedirectToAction("Index", "Home");

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return NotFound($"کاربری با شناسه '{email}' یافت نشد.");

            model.Email = email;
            // Once you add a real email sender, you should remove this code that lets you confirm the account
            model.DisplayConfirmAccountLink = true;
            if (model.DisplayConfirmAccountLink)
            {
                var userId = await _userManager.GetUserIdAsync(user);
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                model.EmailConfirmationUrl = Url.Action(
                    "ConfirmEmail",
                    "Account",
                    new { area = "Identity", userId, code, returnUrl },
                    Request.Scheme);
            }

            return View(model);
        }

        [AllowAnonymous]
        public async Task<IActionResult> ResendEmailConfirmation()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ResendEmailConfirmation(ResendEmailConfirmationModel model)
        {
            if (!ModelState.IsValid) return View();

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Verification email sent. Please check your email.");
                return View();
            }

            var userId = await _userManager.GetUserIdAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Action(
                "ConfirmEmail",
                "Account",
                new { userId, code },
                Request.Scheme);

            await _emailSender.SendEmailAsync(
                model.Email,
                "تایید آدرس ایمیل شما",
                $"لطفا با <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>کلیک کردن این لینک</a> حساب کاربری تان را تایید کنید.");

            ModelState.AddModelError(string.Empty, "ایمیل تایید ارسال شد. لطفا ایمیل تان را چک کنید");

            return View();
        }

        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(string code = null)
        {
            if (code == null) return BadRequest("یک کد برای بازنشانی کلمه عبور مورد نیاز است.");

            var model = new ResetPasswordModel
            {
                Code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code))
            };
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            if (!ModelState.IsValid) return View();

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation");

            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded) return RedirectToAction("ResetPasswordConfirmation");

            foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error.Description);

            return View(model);
        }

        [AllowAnonymous]
        public async Task<IActionResult> ResetPasswordConfirmation()
        {
            return View();
        }
    }
}