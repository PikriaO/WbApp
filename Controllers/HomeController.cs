using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WbApp.Interfaces;
using WbApp.Models;
using WbApp.Services.Authorisation;
using WbApp.Services.Branches;

namespace WbApp.Controllers
{



    public class HomeController : Controller
    {

        private readonly IWbReadOnlyRepository _read;
        private readonly IWbWriteOnlyRepository _write;
        private readonly ILogger<HomeController> _logger;
        public string _lang;


        public HomeController(ILogger<HomeController> logger, IWbReadOnlyRepository read, IWbWriteOnlyRepository write)
        {

            _logger = logger;
            _logger = logger;
            _read = read;
            _write = write;
            _lang = Thread.CurrentThread.CurrentCulture.ToString().ToUpper();
            _lang.Replace("{", "");
            _lang.Replace("}", "");
        }

        public IActionResult ChangeLanguage(string language, string redirectUrl)
        {

            if (!string.IsNullOrEmpty(language))
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(language);
                Thread.CurrentThread.CurrentCulture = new CultureInfo(language);
            }
            HttpContext.Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(language, language)),
          new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1), IsEssential = true });

            return Redirect(redirectUrl);
            //  return Redirect("/home/index");
        }

        public IActionResult  Index(string language, string redirectUrl)
        {
            
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Login(string lang )
        {
            ActionResultModel branch = new ActionResultModel();
            branch = await _read.GetBranches(new GetBranchesCommand() { lang=lang });
            return View(branch);
           
        }

        [HttpPost]
        public async Task<IActionResult> Login(AuthorisationQuery a, string ReturnUrl)
        {

            AuthorisationQuery admin = new AuthorisationQuery();

            var res = await _read.Authrisation(a);

            if (res.ActionResultCode == 200)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, res.OperatorName),
                    new Claim(ClaimTypes.UserData, Convert.ToString(res.UserID)),
                    new Claim(ClaimTypes.Locality, a.BranchName),
                    new Claim(ClaimTypes.Sid, Convert.ToString(a.BranchID)),
                    new Claim(ClaimTypes.Role, Convert.ToString(res.RoleID)),
                };

                var claimsIdentity = new ClaimsIdentity(claims, "Login");

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
                return Redirect(ReturnUrl == null ? "/Home/Index" : ReturnUrl);
            }
            else
                return View(res);
        }

        [HttpPost]
        public IActionResult Logout(string username, string password, string ReturnUrl)
        {
            return Redirect(ReturnUrl == null ? "/Home/Login" : ReturnUrl);

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
    }
}
