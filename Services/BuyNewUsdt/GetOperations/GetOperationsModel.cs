using System;
using System.Collections.Generic;
using static WbApp.Services.CashDesk.VipCashDesk.VipCashDeskModel;

namespace WbApp.Services.BuyNewUsdt.GetOperations
{
    public class GetOperationsModel
    {
        public decimal ToTalAmount { get; set; }
       
        public string CToTalAmount { get; set; }
        public decimal Amount { get; set; }
        public string CAmount { get; set; }
        public decimal FeeAmount { get; set; }
        public string CFeeAmount { get; set; }
        public string BranchName { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int BranchID { get; set; }
        public string AccountNumber { get; set; }
        public string TrAccount { get; set; }
        public IEnumerable<Transaction> transactions { get; set; }
        public IEnumerable<VipTransaction> viptran { get; set; }
        public class Transaction
        {
            public int OperationID { get; set; }
            public DateTime OperationDate { get; set; }
            public int ClientID { get; set; }
            public decimal Amount { get; set; }
            public string CAmount { get; set; }
            public string Currency { get; set; }
            public int Status { get; set; }
            public string StatusName { get; set; }
            public string OperatorName { get; set; }
            public string BranchName { get; set; }
            public string PersonalID { get; set; }
            public string Inn { get; set; }
            public string OrgName { get; set; }
            public string ConvertedAmount { get; set; }
            public string Name { get; set; }
            public string Surname { get; set; }

        }
    }
}
