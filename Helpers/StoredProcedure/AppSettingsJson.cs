using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WbApp.Helpers.StoredProcedure
{
    public static class AppSettingsJson
    {
        public static string ApplicationExeDirectory()
        {
            var location = System.Reflection.Assembly.GetExecutingAssembly().Location;
            var appRoot = Path.GetDirectoryName(location);
            return appRoot;
        }
        public static IConfigurationRoot GetAppSettings()
        {
            string applicationExeDirectory = ApplicationExeDirectory();
            var configBuilder = new ConfigurationBuilder()
               .SetBasePath(applicationExeDirectory)
               .AddJsonFile("appsettings.json", optional: true);
            return configBuilder.Build();
        }
    }
}
