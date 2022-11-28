using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WbApp.Models;
using WbApp.Services.Authorisation;
using WbApp.Services.BuyNewUsdt.MakeOperation;
using WbApp.Services.CloseFillCashDesk;
using WbApp.Services.ConfirmOperation;
using WbApp.Services.MakeOperation;
using WbApp.Services.MakeOperationDetails;
using WbApp.Services.USDTRates;
using WbApp.Services.UserRegistration;
using WbApp.Services.VouchersTracking.AddUpdateVouchers;

namespace WbApp.Interfaces
{
   public interface IWbWriteOnlyRepository
    {
        Task<MakeOperationModel> MakeOperation(MakeOperationCommand a);
        Task<MakeOperationDetailsModel> MakeOperationDetails(MakeOperationDetailsCommand a);
        Task<ConfirmOperationModel> ConfirmOperation(ConfirmOperationCommand a);
        Task<CashDeskOperationModel> CloseFillCashDesk(CashDeskOperationCommand a);
        Task<AddVouchersModel> AddNewVouchers(VouchersList a);
        Task<UsdtRatesModel> UpdateUsdtRate(UsdtRatesCommand a);
        Task<UserRegistrationModel> UserRegistration(UserRegistrationQuery a);
        Task<MakeNewUsdtOperationModel> BuyNewUsdt(MakeNewUsdtOperationQuery a);
    }
}
