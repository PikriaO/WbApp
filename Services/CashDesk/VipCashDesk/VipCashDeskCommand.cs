using MediatR;
using System;

namespace WbApp.Services.CashDesk.VipCashDesk
{
    public class VipCashDeskCommand : IRequest<VipCashDeskModel>
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Lang { get; set; }
        public int? BranchID { get; set; }
    }
}
