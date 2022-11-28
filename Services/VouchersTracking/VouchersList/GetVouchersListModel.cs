using System;
using System.Collections;
using System.Collections.Generic;

namespace WbApp.Services.VouchersTracking.VouchersList
{
    public class GetVouchersListModel
    {
        public string ActionResultMsg { get; set; }

        public IEnumerable<NonUsedVoucher> voucher { get; set; }
        public IEnumerable<VoucherFile> voucherFiles { get; set; }

        public class NonUsedVoucher
        {
            public string Code { get; set; }
            public decimal Amount { get; set; }
            public string FIleName { get; set; }
            public DateTime CreateDate { get; set; }
            public string BranchName { get; set; }
        
        }
        public class VoucherFile
        {
            public string FIleName { get; set; }
            public decimal Amount { get; set; }
            public DateTime CreateDate { get; set; }
            public string BranchName { get; set; }
            public int Count { get; set; }
        }
    }
}
