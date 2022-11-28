using MediatR;

namespace WbApp.Services.CheckVoucher
{
    public class CheckVoucherQuery:IRequest<CheckVoucherModel>
    {
        public string VoucherCode { get; set; }
        public string Lang { get; set; }
    }
}
