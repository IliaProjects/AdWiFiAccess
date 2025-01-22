using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GWA.Classes;
using GWA.Data;
using GWA.Data.Models;
using GWA.DataDashboard;
using GWA.Models.AccountViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace GWA.Controllers
{
    [Authorize]
    public class AccountController : GWAController
    {
        private DashboardDbContext _db;
        private readonly ILogger<AccountController> _logger;
        private readonly IStringLocalizer<AccountController> _localizer;
        private readonly IStringLocalizer<SharedResources> _sharedLocalizer;
        private readonly UserManager<ApplicationUser> _usermanager;
        private readonly SignInManager<ApplicationUser> _signInManager;


        public AccountController( IStringLocalizer<SharedResources> sharedLocalizer, IStringLocalizer<AccountController> localizer,
            ILogger<AccountController> logger, SignInManager<ApplicationUser> signInManager, DashboardDbContext db, UserManager<ApplicationUser> usermanager, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _usermanager = usermanager;
            _logger = logger;
            _signInManager = signInManager;
            _db = db;
            _localizer = localizer;
            _sharedLocalizer = sharedLocalizer;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            if (User != null && User.Identity != null && User.Identity.IsAuthenticated)
                return new RedirectToActionResult("Home", "Index", null);
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            String error = "";

            ViewData["ReturnUrl"] = returnUrl;

            try
            {
                if (ModelState.IsValid)
                {
                    var user = await _usermanager.FindByNameAsync(model.Login);

                    if (user == null)
                    {
                        user = await _usermanager.FindByEmailAsync(model.Login);
                    }

                    if (user == null)
                    {
                        error = _localizer["Incorrect username or password"].Value;
                    }
                    else
                    {
                        user.RememberMe = model.RememberMe;

                        var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: false);

                        if (!result.Succeeded)
                        {
                            if (result.IsLockedOut)
                            {
                                error = _localizer["Your account is locked out. Contact administrator"].Value;
                            }

                            else
                            {
                                error = _localizer["Incorrect username or password"].Value;
                            }
                        }
                        else
                        {
                            _db.UserConnection.Add(new UserConnection
                            {
                                IsOnline = false, // специально делаем, чтобы убедится, когда браузер установит соединение с Websocket, внутри WebSocketMiddleware сделаем update на true
                                SessionId = HttpContext.Session.Id,
                                IsRemember = model.RememberMe,
                                UserId = user.Id,
                                UserIp = HttpContext.Connection.RemoteIpAddress.ToString(),
                                UserAgent = HttpContext.Request.Headers["User-Agent"].ToString(),
                                LastRequestAction = "Login",
                                LastRequestController = "AccountController",
                                LastRequestDate = DateTime.Now,
                                LastRequestPostParams = String.Empty,
                                LastRequestRawUrl = returnUrl ?? String.Empty,
                            });
                            await _db.SaveChangesAsync();
                        }
                    }
                }
                else
                    error = _sharedLocalizer["bad_input_params"].Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(Utils.GetFullError(ex));
                error = _sharedLocalizer["Internal server error"].Value;
            }

            return Json(new
            {
                error = error,
                redirect = (returnUrl == null) ? "" : returnUrl
            });
        }

        [HttpGet]
        async public Task<IActionResult> Logout()
        {
            var conn = _db.UserConnection.FirstOrDefault(s => s.SessionId == User.Identity.GetSessionId());
            if (conn != null)
            {
                _db.UserConnection.Remove(conn);
                await _db.SaveChangesAsync();
            }

            Request.HttpContext.Session.Clear();
            Request.HttpContext.Response.Cookies.Delete(".AspNetCore.Session");

            await _signInManager.SignOutAsync();

            return RedirectToAction(nameof(HomeController.Index), "Home");
        }
    }
}