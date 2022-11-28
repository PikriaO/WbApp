using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using WbApp.Models;
using WbApp.Services.Authorisation;
using WbApp.Services.Branches;
using WbApp.Services.BuyNewUsdt.ConfirmUsdtCashIn;
using WbApp.Services.BuyNewUsdt.GetCode;
using WbApp.Services.BuyNewUsdt.GetOperations;
using WbApp.Services.CalcUsdtPrice;
using WbApp.Services.CashDesk;
using WbApp.Services.CashDesk.FillCloseTransactions;
using WbApp.Services.CashDesk.VipCashDesk;
using WbApp.Services.CashDeskTransactions;
using WbApp.Services.CheckPersonalID;
using WbApp.Services.CheckVoucher;
using WbApp.Services.GetPaymentDetails;
using WbApp.Services.GetUser;
using WbApp.Services.USDTRates;
using WbApp.Services.VouchersTracking.VouchersList;

namespace WbApp.Interfaces
{
    public interface IWbReadOnlyRepository
    {
        Task<ActionResultModel> Authrisation(AuthorisationQuery a);
        Task<CheckVoucherModel> CheckVoucher(CheckVoucherQuery a);
        Task<PersonDataModel> CheckPersonalID(CheckPersonQuery a);
        Task<PaymentDetailsModel> GetPaymentDetails(PaymentDetailsQuery a);
        Task<CashDeskTransactionsModel> GetCashDeskTransactions(CashDeskTransactionsQuery a);
        Task<CashDeskTransactionsModel> GetCashDeskBalance(CashDeskTransactionsQuery a);
        Task<GetVouchersListModel> GetVouchersList(GetVouchersQuery a);
        Task<TransactionsModel> GetTransactions(TransactionsQuery a);
        Task<UsdtRatesModel> GetusdtRates(UsdtRatesCommand a);
        Task<ActionResultModel> GetBranches(GetBranchesCommand a);
        Task<GetUserModel> GetUser(GetUserCommand a);
        Task<CalcResult> CalcPrice(CalcCommand a);
        Task<GetOperationsModel> GetOperations(GetOperationsCommand a);
        Task<ConfirmCashInModel> ConfirmUsdtCashIn(ConfirmCashInCommand a);
        Task<string> GenerateQr(string Code);
        Task<GetCodeModel> GetCode(GetCodeCommand a);
        Task<VipCashDeskModel> GetVipCashDesk(VipCashDeskCommand a);
    }
}
