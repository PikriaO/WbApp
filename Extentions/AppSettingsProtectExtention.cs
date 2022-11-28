using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WbApp.Extentions
{
    public static class AppSettingsProtectExtention
    {
        public static string Unprotect(this string data, string purpose)
        {

             
            var localappdata = Environment.GetEnvironmentVariable("LocalAppData");

            var datapr = Microsoft.AspNetCore.DataProtection.DataProtectionProvider.Create(new System.IO.DirectoryInfo(@$"{localappdata}\ASP.NET\DataProtection-Keys"));

            var pr = datapr.CreateProtector(purpose);

            return Encoding.ASCII.GetString(pr.Unprotect(System.Convert.FromBase64String(data)));

        }
    }
}
