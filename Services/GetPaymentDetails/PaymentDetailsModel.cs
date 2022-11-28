using System;
using System.Collections.Generic;

namespace WbApp.Services.GetPaymentDetails
{
    public class PaymentDetailsModel
    {
        public string start { get; set; }
        public string end { get; set; }
        public decimal? CashAmount { get; set; }
        public string  CAmount { get; set; }
        public decimal? TerminalAmount { get; set; }
        public string TAmount { get; set; }
        public decimal? BankAmount { get; set; }
        public string BAmount { get; set; }
        public decimal? SumAmount { get; set; }
        public string SAmount { get; set; }
        public string OperatorName { get; set; }
        public string BranchName { get; set; }
        public string Comment { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Phone { get; set; }
        public string BranchAddress { get; set; }
        public DateTime BirthDate { get; set; }
        public DateTime CreateDate { get; set; }
        public string PersonID { get; set; }
        public int Count { get; set; }
        public int BranchID { get; set; }
        public string CreateMonth { get; set; }
        public IEnumerable<Detail> details { get; set; }

        public class Detail
        {
            public int PaymentID { get; set; }
            public string Code { get; set; }
            public string Ccy { get; set; }
            public int Count { get; set; }
            public decimal Amount { get; set; }
            public string ConvertedAmount { get; set; }

        }
    } 
  
}
