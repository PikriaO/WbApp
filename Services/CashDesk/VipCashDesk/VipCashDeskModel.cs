using System;
using System.Collections;
using System.Collections.Generic;

namespace WbApp.Services.CashDesk.VipCashDesk
{
    public class VipCashDeskModel
    {
        public decimal ToTalAmount { get; set; }
        public string CToTalAmount { get; set; }
        public decimal Amount { get; set; }
        public string CAmount { get; set; }
        public decimal FeeAmount { get; set; }
        public string CFeeAmount { get; set; }
        public string AccountNumber { get; set; }
        public string TrAccount { get; set; }
        public IEnumerable<VipTransaction> vipTrans { get; set; }
        public class VipTransaction
        {
            public int CashInID { get; set; }
            public DateTime OperationDate { get; set; }
            public string AccountNumber { get; set; }
            public string TrAccount { get; set; }
            public decimal? Amount { get; set; }
            public string CAmount { get; set; }
            public decimal? FeeAmount { get; set; }
            public string CFeeAmount { get; set; }
            public decimal? ToTalAmount { get; set; }
            public string CToTalAmount { get; set; }
            public string Name { get; set; }
            public string Surname { get; set; }
            public string Phone { get; set; }
            public int OperationID { get; set; }

        }
    }
}
