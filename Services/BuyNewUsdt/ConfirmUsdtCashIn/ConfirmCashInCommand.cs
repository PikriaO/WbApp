using MediatR;

namespace WbApp.Services.BuyNewUsdt.ConfirmUsdtCashIn
{
    public class ConfirmCashInCommand:IRequest<ConfirmCashInModel>
    {
        public int OperationID { get; set; }
        public int BranchID { get; set; }
        public int OperatorID { get; set; }
        public string Lang { get; set; }
    }
}
