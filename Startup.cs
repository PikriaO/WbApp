using DinkToPdf;
using DinkToPdf.Contracts;
using FluentAssertions.Common;
using MediatR;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WbApp.Interfaces;
using WbApp.Services;
namespace WbApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddMediatR(Assembly.GetExecutingAssembly());
            services.AddScoped<IWbReadOnlyRepository, WbRepository>();
            services.AddScoped<IWbWriteOnlyRepository, WbRepository>();
            services.AddScoped<IViewRenderService, ViewRenderService>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));
            services.AddLocalization(options => options.ResourcesPath = "Resource");

            services.AddMvc()
                .AddViewLocalization(x=>{ x.ResourcesPath = "Resource"; })
                .AddDataAnnotationsLocalization();



            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
             .AddCookie(options =>
             {
                 options.LoginPath = "/Home/Login";
             });
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

          
            IList<CultureInfo> supportedCultures = new List<CultureInfo>
            {

                 new CultureInfo("ka"),
                 new CultureInfo("en")

             };
            app.UseCookiePolicy(
         new CookiePolicyOptions
         {
             Secure = CookieSecurePolicy.None
         });
            //app.UseRequestLocalization(new RequestLocalizationOptions
            //{
            //    ApplyCurrentCultureToResponseHeaders = true
            //});
           
            app.UseRequestLocalization(new RequestLocalizationOptions
            {

                ApplyCurrentCultureToResponseHeaders = true,
                DefaultRequestCulture = new RequestCulture("en"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures,
                               RequestCultureProviders = new List<IRequestCultureProvider>
                     {
                    new QueryStringRequestCultureProvider(),
                 new CookieRequestCultureProvider()
              }

            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Login}/{id?}");
            });

        }
    }
}
