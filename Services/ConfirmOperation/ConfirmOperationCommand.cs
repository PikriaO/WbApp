using MediatR;

namespace WbApp.Services.ConfirmOperation
{
    public class ConfirmOperationCommand:IRequest<ConfirmOperationModel>
    {
        public int PaymentID { get; set; }
        public string Lang { get; set; }
    }
}
