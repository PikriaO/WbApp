using ExcelDataReader;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System;
using System.Threading.Tasks;
using WbApp.Services.VouchersTracking.AddUpdateVouchers;
using WbApp.Services.VouchersTracking.VouchersList;
using WbApp.Models;
using Microsoft.Extensions.Logging;
using System.Threading;
using WbApp.Interfaces;
using Microsoft.AspNetCore.Hosting;
using System.Text;
using DinkToPdf;
using WbApp.Services;
using Microsoft.Graph;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.DependencyInjection;
using WbApp.Helpers;
using DinkToPdf.Contracts;
using Microsoft.VisualBasic.FileIO;
using RestSharp;
using QRCoder;
using System.Drawing;
using iTextSharp.text.pdf.qrcode;
using QRCode = QRCoder.QRCode;
using System.Drawing.Imaging;
using Oracle.ManagedDataAccess.Client;
using WbApp.Helpers.StoredProcedure;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using WbApp.Services.USDTRates;
using System.Numerics;

namespace WbApp.Controllers
{
    [Authorize]
    public class SettingsController : Controller
    {

        private IMediator _mediator;
        protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>();
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly ILogger<SettingsController> _logger;
        private readonly IStringLocalizer _localizer;
        private readonly IConverter _converter;
        private readonly IWbReadOnlyRepository _read;
        private readonly IConfiguration _iconfiguration;
        private readonly IViewRenderService _viewRenderService;
        public string _lang;
        public SettingsController(IConverter converter, ILogger<SettingsController> logger, IConfiguration configuration, IWbReadOnlyRepository read, IViewRenderService viewRenderService, IHttpContextAccessor contextAccessor, IHostingEnvironment hostingEnvironment)
        {
            _iconfiguration = configuration;
            _logger = logger;
            _hostingEnvironment = hostingEnvironment;
            _converter = converter;
            _viewRenderService = viewRenderService;
            _read = read;
            _lang = Thread.CurrentThread.CurrentCulture.ToString().ToUpper();
            _lang.Replace("{", "");
            _lang.Replace("}", "");
            IStringLocalizerFactory factory = contextAccessor.HttpContext.RequestServices.GetService<IStringLocalizerFactory>();
        }
        public IActionResult GenerateVouchers()
        {
            return View("~/Views/Settings/GenerateVouchers.cshtml");

        }
        public IActionResult PrintVouchers()
        {
            var data = new PdfData();

            data.Usdt = 50;
            data.QrData = "data:image/jpeg;base64,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";
            return View("~/Views/Pdf/PrintVouchers.cshtml", data);

        }


        [HttpPost]
        public async Task<FileContentResult> ExportToPdf(IFormFile file)
        {
            var counter = 0;
            var fileName = "";
            string siteUrl = _iconfiguration.GetSection("ConnectionStrings").GetSection("Siteurl").Value;
            var data = new List<PdfData>();
            try
            {
                fileName = Path.GetFileName(file.FileName);
                var filePath = Path.Combine(_hostingEnvironment.WebRootPath, @"uploads", fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                System.Data.DataRowCollection result;
                using (var stream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read))
                {
                    using (var reader = ExcelReaderFactory.CreateReader(stream, new ExcelReaderConfiguration()
                    {
                        FallbackEncoding = Encoding.GetEncoding(1252),
                    }))
                    {
                        do
                        {
                            while (reader.Read())
                            {

                            }
                        } while (reader.NextResult());


                        result = reader.AsDataSet().Tables[0].DefaultView.Table.Rows;

                    }
                    var r1 = result.Cast<DataRow>().Skip(1);

                    foreach (System.Data.DataRow item in r1)
                    {
                        counter++;
                        var qr = _read.GenerateQr(Convert.ToString(item.ItemArray[0]));

                        try
                        {
                            data.Add(new PdfData()
                            {

                                ImgUrl = siteUrl,
                                VoucherCode = Convert.ToString(item.ItemArray[0]),
                                QrData = "data:image/jpeg;base64," + qr.Result.ToString(),
                                VoucherPass = Convert.ToString(item.ItemArray[1]),
                                Usdt = Convert.ToInt32(item.ItemArray[2]),
                                count = counter,
                            });
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                    System.IO.File.Delete(filePath);
                }
            }
            catch (Exception e)
            {

            }

            string html = await _viewRenderService.RenderToStringAsync("QRVouchers", data);

            var globalSettings = new GlobalSettings
            {
                // ColorMode = ColorMode.Color,
                Orientation = Orientation.Landscape,
                PaperSize = PaperKind.A3Transverse,
                Outline = false,
                Margins = new MarginSettings { Top = 5, Bottom = 5, Right = 5, Left = 5 },

                DocumentTitle = "Document",


            };
            //pdf settings
            var objectSettings = new ObjectSettings
            {
                PagesCount = true,
                HtmlContent = html,
                //        WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = Path.Combine(_hostingEnvironment.WebRootPath, "css", "dashboard_style.css") },
                //for header setting
                //  HeaderSettings = { FontSize = 9, Center = _localizer["ElectronicStatement"] },
                //for footer settings
                //FooterSettings = { FontName = "Arial", FontSize = 9, Line = true, Left = "<p style='color:blue;'>www.payunicard.ge</p>" }
            };

            var pdf = new HtmlToPdfDocument
            {
                GlobalSettings = globalSettings,
                Objects = { objectSettings }
            };

            //Create pdf file
            CustomAssemblyLoadContext context = new CustomAssemblyLoadContext();
            context.LoadUnmanagedLibrary(Path.Combine(_hostingEnvironment.ContentRootPath, "libwkhtmltox.dll"));

            var bytes = _converter.Convert(pdf);
            var name = $"{Guid.NewGuid().ToString()}.pdf";

            return new FileContentResult(bytes, "application/pdf") { FileDownloadName = "Vouchers-back side " + fileName + ".pdf" };
        }

        [HttpPost]
        public async Task<FileContentResult> ExportToPdfBackSide(IFormFile file, int color)
        {
            var fileName = "";
            string siteUrl = _iconfiguration.GetSection("ConnectionStrings").GetSection("Siteurl").Value;
            var data = new List<PdfData>();
            try
            {
                fileName = Path.GetFileName(file.FileName);
                var filePath = Path.Combine(_hostingEnvironment.WebRootPath, @"uploads", fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                System.Data.DataRowCollection result;
                using (var stream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read))
                {
                    using (var reader = ExcelReaderFactory.CreateReader(stream, new ExcelReaderConfiguration()
                    {
                        FallbackEncoding = Encoding.GetEncoding(1252),
                    }))
                    {
                        do
                        {
                            while (reader.Read())
                            {

                            }
                        } while (reader.NextResult());


                        result = reader.AsDataSet().Tables[0].DefaultView.Table.Rows;

                    }
                    var r1 = result.Cast<DataRow>().Skip(1);

                    foreach (System.Data.DataRow item in r1)
                    {
                        try
                        {
                            data.Add(new PdfData()
                            {
                                Usdt = Convert.ToInt32(item.ItemArray[2]),
                                ImgUrl = siteUrl,
                            });
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                    System.IO.File.Delete(filePath);
                }
            }
            catch (Exception e)
            {

            }
            string html;
           // if (color == 1)
           // {
                html = await _viewRenderService.RenderToStringAsync("QRVouchersBackSide", data);
          //  }
            //else
            //{
            //    html = await _viewRenderService.RenderToStringAsync("QRVouchersBackSideWhite", data);
            //}
            var globalSettings = new GlobalSettings
            {
                // ColorMode = ColorMode.Color,
                Orientation = Orientation.Landscape,                
                PaperSize = PaperKind.A3Transverse,
                Outline = false,
                 Margins = new MarginSettings { Top = 5, Bottom = 5, Right = 5, Left = 5 },
                DocumentTitle = "Document", 
            };
            //pdf settings
            var objectSettings = new ObjectSettings
            {
                PagesCount = true,
                HtmlContent = html,

                //        WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = Path.Combine(_hostingEnvironment.WebRootPath, "css", "dashboard_style.css") },
                //for header setting
                //  HeaderSettings = { FontSize = 9, Center = _localizer["ElectronicStatement"] },
                //for footer settings
                //FooterSettings = { FontName = "Arial", FontSize = 9, Line = true, Left = "<p style='color:blue;'>www.payunicard.ge</p>" }
            };

            var pdf = new HtmlToPdfDocument
            {
                GlobalSettings = globalSettings,
                Objects = { objectSettings }
            };

            //Create pdf file
            CustomAssemblyLoadContext context = new CustomAssemblyLoadContext();
            context.LoadUnmanagedLibrary(Path.Combine(_hostingEnvironment.ContentRootPath, "libwkhtmltox.dll"));

            var bytes = _converter.Convert(pdf);
            var name = $"{Guid.NewGuid().ToString()}.pdf";

            return new FileContentResult(bytes, "application/pdf") { FileDownloadName = "Vouchers-front side" + fileName + ".pdf" };
        }

        [HttpGet]
        public async Task<IActionResult> UsdtRates(UsdtRatesCommand a)
        {
            a.Lang = _lang;
            a.ActionType = 1;
            var data = await Mediator.Send(a);
         
            data.Mode = a.Mode;
            return View("~/Views/Settings/VoucherPrice.cshtml", data);
        }

        [HttpPost]
        public async Task<UsdtRatesModel> UpdateUsdtRates(UsdtRatesCommand a)
        {
            a.Lang = _lang;
            var data = await Mediator.Send(a);
            return data;
        }
    }


}


