using MediatR;

namespace WbApp.Services.CloseFillCashDesk
{
    public class CashDeskOperationCommand : IRequest<CashDeskOperationModel>
    {
        public int OpType { get; set; }
        public int OperatorID { get; set; }
        public int BranchID { get; set; }
        public decimal Amount { get; set; }
        public string Lang { get; set; }
    }
}
