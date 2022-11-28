using MediatR;
using System;
using WbApp.Services.CashDesk;

namespace WbApp.Services.CashDeskTransactions
{
    public class CashDeskTransactionsQuery:IRequest<CashDeskTransactionsModel>
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int BranchID { get; set; }
        public string Lang { get; set; }
    }
}
