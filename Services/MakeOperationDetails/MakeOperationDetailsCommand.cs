using MediatR;

namespace WbApp.Services.MakeOperationDetails
{
    public class MakeOperationDetailsCommand:IRequest<MakeOperationDetailsModel>    
    {
        public string VoucherCode { get; set; }
        public int PaymentID { get; set; }
        public string Lang { get; set; }
    }
}
