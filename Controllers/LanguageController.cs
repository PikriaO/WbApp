using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Security.Claims;
using System.Threading;
using WbApp.Interfaces;

namespace WbApp.Controllers
{
    [Authorize]
    public class LanguageController : Controller
    {
      
        private readonly IStringLocalizer _localizer;

        public LanguageController(IStringLocalizerFactory stringLocalizer)
        {
            var type = typeof(SharedResource);
            var assemblyName = new AssemblyName(type.GetTypeInfo().Assembly.FullName);
            _localizer = stringLocalizer.Create("SharedResource", assemblyName.Name);
        }
        public IActionResult Index(string language,string redirectUrl)
        {

            if (!string.IsNullOrEmpty(language))
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(language);
                Thread.CurrentThread.CurrentCulture = new CultureInfo(language);
            } 
            HttpContext.Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(new RequestCulture("en", language)),
            new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1), IsEssential = true });          
        

            return Redirect(redirectUrl);
            //  return Redirect("/home/index");
        }
    }
} 
