using MediatR;

namespace WbApp.Services.BuyNewUsdt.MakeOperation
{
    public class MakeNewUsdtOperationQuery:IRequest<MakeNewUsdtOperationModel>
    {
        public int ClientID { get; set; }
        public decimal Amount { get; set; }
        public decimal FeeAmount { get; set; }
        public decimal Usdt { get; set; }
        public int OperatorID { get; set; }
        public int BranchID { get; set; }
        public string Lang { get; set; }
        
    }
}
