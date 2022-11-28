using MediatR;
using System.Collections.Generic;

namespace WbApp.Services.VouchersTracking.AddUpdateVouchers
{
    public class AddVouchersCommand: IRequest<AddVouchersModel>
    {
        public string Lang { get; set; }
        public IEnumerable<VouchersData> vouchersData { get; set; }

    }
    public class VouchersData
    {

        public string Code { get; set; }
        public string Lang { get; set; }
        public decimal? Amount { get; set; }
        public int BranchID { get; set; }
        public string FileName { get; set; }
        public int Mode { get; set; }
        public int Usdt { get; set; }


     
        
    }
}
