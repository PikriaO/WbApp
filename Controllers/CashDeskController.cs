using ExcelDataReader;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WbApp.Interfaces;
using WbApp.Services.BuyNewUsdt.ConfirmUsdtCashIn;
using WbApp.Services.BuyNewUsdt.GetCode;
using WbApp.Services.BuyNewUsdt.GetOperations;
using WbApp.Services.BuyNewUsdt.MakeOperation;
using WbApp.Services.CalcUsdtPrice;
using WbApp.Services.CashDeskTransactions;
using WbApp.Services.CheckPersonalID;
using WbApp.Services.CheckVoucher;
using WbApp.Services.CloseFillCashDesk;
using WbApp.Services.GetPaymentDetails;
using WbApp.Services.GetUser;
using WbApp.Services.MakeOperation;
using WbApp.Services.Report;
using WbApp.Services.UserRegistration;
using WbApp.Services.VouchersTracking.AddUpdateVouchers;
using WbApp.Services.VouchersTracking.VouchersList;

namespace WbApp.Controllers
{
    [Authorize]
    public class CashDeskController : Controller
    {
        private IMediator _mediator;
        private readonly IHostingEnvironment _hostingEnvironment;
        protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>();
        private readonly IWbReadOnlyRepository _read;
        private readonly IWbWriteOnlyRepository _write;
        private readonly ILogger<CashDeskController> _logger;
        public string _lang;

        public CashDeskController(ILogger<CashDeskController> logger, IWbReadOnlyRepository read, IWbWriteOnlyRepository write, IHostingEnvironment hostingEnvironment)
        {
            _lang = Thread.CurrentThread.CurrentCulture.ToString().ToUpper();
            _lang.Replace("{", "");
            _lang.Replace("}", "");
            _logger = logger;
            _read = read;
            _write = write;
            _hostingEnvironment = hostingEnvironment;

        }
        public IActionResult UsersProfile()
        {
            return View("~/Views/User/UsersProfile.cshtml");
        }

        public ActionResult Vouchers(int id)
        {
            return View();
        }

        [HttpGet]
        public async Task<CheckVoucherModel> CheckVoucher(string voucherCode)
        {

            var data = new CheckVoucherModel();
            try
            {
                data = await _read.CheckVoucher(new Services.CheckVoucher.CheckVoucherQuery { VoucherCode = voucherCode, Lang = _lang });

            }
            catch (Exception e)
            {
                return new CheckVoucherModel()
                {
                    ActionResultMsg = e.Message,
                    ActionResultCode = 500

                };
            }
            return data;
        }

        [HttpGet]
        public async Task<PersonDataModel> CheckPerson(CheckPersonQuery a)
        {
            a.Lang = _lang;
            var data = new PersonDataModel();
            try
            {
                data = await _read.CheckPersonalID(a);

            }
            catch (Exception e)
            {
                return new PersonDataModel()
                {
                    ActionResultMsg = e.Message,
                    ActionResultCode = 500

                };
            }
            return data;
        }
        [HttpPost]
        public async Task<MakeOperationModel> MakeOperation(MakeOperationCommand a)
        {
            a.Lang = _lang;
            MakeOperationModel data = new MakeOperationModel();
            a.BranchID = Convert.ToInt32(User.Claims.Where(c => c.Type == ClaimTypes.Sid)
                   .Select(c => c.Value).FirstOrDefault());
            a.OperatorID = Convert.ToInt32(User.Claims.Where(c => c.Type == ClaimTypes.UserData)
                  .Select(c => c.Value).FirstOrDefault());
            data = await Mediator.Send(a);

            return data;

        }

        [HttpGet]
        public async Task<IActionResult> GetPaymentDetails(int PaymentID, int BranchID, DateTime Start, DateTime End)
        {

            var data = await Mediator.Send(new PaymentDetailsQuery() { PaymentID = PaymentID, Lang = _lang });
            data.BranchID = BranchID;
            data.start = Start.ToString("yyyy-MM-dd");
            data.end = End.ToString("yyyy-MM-dd");
            return View("~/Views/CashDesk/PaymentDetails.cshtml", data);
        }

        [HttpGet]
        public async Task<IActionResult> GetCashDeskTransactions(CashDeskTransactionsQuery a)
        {
            a.Lang = _lang;
            var data = await Mediator.Send(a);
            data.BranchID = a.BranchID;
            data.BranchName = Convert.ToString(User.Claims.Where(c => c.Type == ClaimTypes.Locality)
                    .Select(c => c.Value).FirstOrDefault());
            data.StartDate = a.StartDate == null ? DateTime.Now.Date.ToString("yyyy-MM-dd") : a.StartDate?.ToString("yyyy-MM-dd");
            data.EndDate = a.EndDate == null ? DateTime.Now.Date.ToString("yyyy-MM-dd") : a.EndDate?.ToString("yyyy-MM-dd");

            return View("~/Views/CashDesk/CashDeskTransactions.cshtml", data);
        }


        [HttpPost]
        public async Task<CashDeskOperationModel> CloseFillCashDesk(CashDeskOperationCommand a)
        {
            a.Lang = _lang;
            a.BranchID = Convert.ToInt32(User.Claims.Where(c => c.Type == ClaimTypes.Sid)
                 .Select(c => c.Value).FirstOrDefault());
            a.OperatorID = Convert.ToInt32(User.Claims.Where(c => c.Type == ClaimTypes.UserData)
                  .Select(c => c.Value).FirstOrDefault());

            var data = await Mediator.Send(a);

            return data;
        }

        [HttpGet]
        public async Task<IActionResult> VouchersPool(GetVouchersQuery a)
        {
            a.Lang = _lang;
            var data = new GetVouchersListModel();
            data = await Mediator.Send(a);

            return View("~/Views/CashDesk/VouchersPool.cshtml", data);
        }

        [HttpGet]
        public async Task<IActionResult> GetCardsByFile(GetVouchersQuery a)
        {
            a.Lang = _lang;
            var data = new GetVouchersListModel();
            data = await Mediator.Send(a);
            return View("~/Views/CashDesk/VouchersbyFile.cshtml", data);
        }
        [HttpPost]
        public async Task<IActionResult> ReadVouchersExcell(IFormFile file, int BranchID)
        {
            try
            {
                var list = new List<VouchersData>();

                var fileName = Path.GetFileName(file.FileName);
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


                        // The result of each spreadsheet is in result.Tables
                    }
                    var r1 = result.Cast<DataRow>().Skip(1);



                    foreach (System.Data.DataRow item in r1)
                    {
                        try
                        {
                            list.Add(new VouchersData()
                            {
                                Code = Convert.ToString(item.ItemArray[0]),
                                Usdt = Convert.ToInt32(item.ItemArray[1]),
                                Amount = 0,// Convert.ToInt32(item.ItemArray[2]),
                                BranchID = BranchID,
                                FileName = file.FileName,
                                Mode = 0,
                                Lang = _lang

                            });
                        }
                        catch (Exception ex)
                        {

                            var data = await Mediator.Send(new GetVouchersQuery() { });

                            ViewBag.Message = "დაფიქსირდა შეცდომა";
                            return View("~/Views/CashDesk/VouchersPool.cshtml", data);
                        }

                    }
                    try
                    {
                        var res = await Mediator.Send(new AddVouchersCommand() { vouchersData = list });
                        var data = await Mediator.Send(new GetVouchersQuery() { });
                        data.ActionResultMsg = res.ActionResultMsg;

                        ViewBag.Message = res.ActionResultMsg;
                        return View("~/Views/CashDesk/VouchersPool.cshtml", data);
                    }
                    catch (Exception ex)
                    {
                        ViewBag.Message = "დაფიქსირდა შეცდომა";
                        return View("~/Views/CashDesk/VouchersPool.cshtml");
                    }


                }
            }
            catch (Exception e)
            {
                ViewBag.Message = e.Message;
                return View("~/Views/CashDesk/VouchersPool.cshtml");
            }
        }

        [HttpPost]
        public async Task<AddVouchersModel> ChangeVouchersBranch(AddVouchersCommand a)
        {
            a.Lang = _lang;
            AddVouchersModel data = new AddVouchersModel();

            data = await Mediator.Send(new AddVouchersCommand() { vouchersData = a.vouchersData });

            return data;

        }

        [HttpGet]
        public async Task<IActionResult> vouchersreport(VouchersReportQuery a)
        {
            a.Lang = _lang;
            var data = new VouchersReportModel();

            data = await Mediator.Send(a);

            return View("~/Views/CashDesk/VouchersReport.cshtml", data);
        }


        [HttpGet]
        public async Task<IActionResult> Customers()
        {
            return View();
        }

        [HttpPost]
        public async Task<UserRegistrationModel> UserRegistration(UserRegistrationQuery a)
        {
            a.Lang = _lang;
            var data = await Mediator.Send(a);
            return data;
        }

        [HttpGet]
        public async Task<IActionResult> GetUser(GetUserCommand a)
        {
            var data = new GetUserModel();
            a.Lang = _lang;
            if (a.PersonalID != null || a.Inn != null)
            {
                data = await Mediator.Send(a);
                data.IsLegal = a.Inn == null ? 0 : 1;
            }
            return View("~/Views/CashDesk/UserProfile.cshtml", data);
        }

        [HttpGet]
        public async Task<IActionResult> GetUsersList(GetUserCommand a)
        {
            a.Lang = _lang;
            var data = await Mediator.Send(a);
            data.IsLegal = a.IsLegal;
            try
            {
                data.users = data.users.Where(t => t.IsLegal == a.IsLegal);
            }
            catch (Exception e)
            {

            }
            return View("~/Views/CashDesk/UsersList.cshtml", data);
        }

        [HttpGet]
        public async Task<CalcResult> CalcUsdt(CalcCommand a)
        {
            var data = await Mediator.Send(a);
            return data;
        }

        [HttpPost]
        public async Task<MakeNewUsdtOperationModel> BuyNewUsdt(MakeNewUsdtOperationQuery a)
        {
            a.Lang = _lang;
            a.BranchID = Convert.ToInt32(User.Claims.Where(c => c.Type == ClaimTypes.Sid)
               .Select(c => c.Value).FirstOrDefault());
            a.OperatorID = Convert.ToInt32(User.Claims.Where(c => c.Type == ClaimTypes.UserData)
                  .Select(c => c.Value).FirstOrDefault());
            var data = await Mediator.Send(a);
            return data;
        }

        [HttpGet]
        public async Task<ConfirmCashInModel> ConfirmCashIn(ConfirmCashInCommand a)
        {
            var data = await Mediator.Send(a);
            return data;
        }

        [HttpGet]
        public async Task<IActionResult> VipCashDesk(GetOperationsCommand a)
        {

            a.BranchID = Convert.ToInt32(User.Claims.Where(c => c.Type == ClaimTypes.Sid)
              .Select(c => c.Value).FirstOrDefault());
            var data = await Mediator.Send(a);
            data.StartDate = a.StartDate?.ToString("yyyy-MM-dd");
            data.EndDate = a.EndDate?.ToString("yyyy-MM-dd");
            data.BranchName = Convert.ToString(User.Claims.Where(c => c.Type == ClaimTypes.Locality)
                    .Select(c => c.Value).FirstOrDefault());
            return View("~/Views/CashDesk/VipAndCorpCashDesk.cshtml", data);
        }

        [HttpGet]
        public async Task<GetCodeModel> GetCode(GetCodeCommand a)
        {
            var data = await Mediator.Send(a);
            data.FormatedDate = data.CreateDate.ToString("dd-MM-yyyy HH:mm");
            return data;
        }
    }
}
