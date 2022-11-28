using MediatR;
using System.Collections.Generic;
using System;
using WbApp.Services.MakeOperation;
using WbApp.Services.VouchersTracking.AddUpdateVouchers;

namespace WbApp.Services.VouchersTracking.ChangeVouchersBranch
{
    public class ChangeVouchersBranchCommand : IRequest<AddVouchersModel>
    {

        public List<VouchersBranch> vouchersBranch { get; set; }

    }
    public class VouchersBranch
    {

        public string Code { get; set; }
        public int BranhID { get; set; }

    }
}