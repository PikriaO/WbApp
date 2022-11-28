using MediatR;

namespace WbApp.Services.VouchersTracking.VouchersList
{
    public class GetVouchersQuery:IRequest<GetVouchersListModel>
    {
        public string FileName { get; set; }
        public string Lang { get; set; }
        public int? BranchID { get; set; }
    }
}
