using System;
using System.Collections;
using System.Collections.Generic;

namespace WbApp.Services.CashDesk.FillCloseTransactions
{
    public class TransactionsModel
    {
        public IEnumerable<Tran> tran { get; set; }

        public class Tran
        {
            public string Operator { get; set; }
            public string OpClass { get; set; }
            public decimal Amount { get; set; }
            public string AmountC { get; set; }
            public string Description { get; set; } 
            public DateTime TranDate { get; set; }
        }
    }
}
