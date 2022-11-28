using System;
using System.Collections.Generic;

namespace WbApp.Services.CashDesk
{
    public class CashDeskTransactionsModel
    {
        public string BranchName { get; set; }
        public decimal TerminalAmount { get; set; }
        public string TAmount { get; set; }
        public int BranchID { get; set; } 
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public decimal BankAmount { get; set; }
        public string BAmount { get; set; }
        public decimal CashAmount { get; set; }
        public string CAmount { get; set; }
        public decimal CashDeskbalance { get; set; }
        public string CBalance { get; set; }
        public string AccountNumber { get; set; }
        public string TrAccount { get; set; }
        public int CountCash { get; set; }
        public int CountTerminal { get; set; }
        public int CountSum { get; set; }
        public int CountBank { get; set; }
        public decimal SumAmount { get; set; } 
        public string SAmount { get; set; }
        public int TranCount { get; set; }
        public int CardCount { get; set; }
        public IEnumerable<Transaction> transactions { get; set; }
        public IEnumerable<CashTransaction> cashTransactions { get; set; }

        public class Transaction
        {
            public decimal? CashAmount { get; set; }
            public string CAmount { get; set; }
            public decimal? TerminalAmount { get; set; }
            public string TAmount { get; set; }
            public decimal? BankAmount { get; set; }
            public string BAmount { get; set; }
            public decimal? SumAmount { get; set; } 
            public string OperatorName { get; set; }
            public string BranchName { get; set; }
            public string Name { get; set; }
            public string Surname { get; set; }
            public int PaymentID { get; set; }
            public DateTime CreateDate { get; set; }

        }
        public class CashTransaction
        {
            public decimal? Amount { get; set; }
            public string CAmount { get; set; } 
            public string Description { get; set; } 
            public string OperatorName { get; set; } 
            public DateTime CreateDate { get; set; }

        }
    }
}
