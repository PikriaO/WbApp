using MediatR;

namespace WbApp.Services.BuyNewUsdt.GetCode
{
    public class GetCodeCommand:IRequest<GetCodeModel>
    {
        public int OperationID { get; set; }
    }
}
