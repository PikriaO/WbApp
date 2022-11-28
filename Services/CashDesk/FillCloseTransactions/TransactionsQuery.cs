using MediatR;
using System;
using System.Drawing.Printing;

namespace WbApp.Services.CashDesk.FillCloseTransactions
{
    public class TransactionsQuery:IRequest<TransactionsModel>
    {
        public int BranchID { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Lang { get; set; }
    }
}
