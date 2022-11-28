using MediatR;

namespace WbApp.Services.GetPaymentDetails
{
    public class PaymentDetailsQuery:IRequest<PaymentDetailsModel>
    {
        public int PaymentID { get; set; }
        public string Lang { get; set; }
    }
}
