using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using WbApp.Interfaces;
using WbApp.Services.ConfirmOperation;
using WbApp.Services.MakeOperationDetails;

namespace WbApp.Services.MakeOperation
{
    public class MakeOperationHandler : IRequestHandler<MakeOperationCommand, MakeOperationModel>
    {
        private readonly IWbReadOnlyRepository _readRepository;
        private readonly IWbWriteOnlyRepository _writeRepository;
        public MakeOperationHandler(IWbReadOnlyRepository readRepository, IWbWriteOnlyRepository writeRepository)
        {
            _readRepository = readRepository;
            _writeRepository = writeRepository;
        }
        public async Task<MakeOperationModel> Handle(MakeOperationCommand request, CancellationToken cancellationToken)
        {

            MakeOperationModel res = new MakeOperationModel();
            ConfirmOperationModel confirm = new ConfirmOperationModel();
            MakeOperationDetailsModel details = new MakeOperationDetailsModel();
 
            try
            {
                res = await _writeRepository.MakeOperation(request);
                if (res.ActionResultCode == 200)
                {

                    foreach (var i in request.vouchers)
                    {
                        details = await _writeRepository.MakeOperationDetails(new Services.MakeOperationDetails.MakeOperationDetailsCommand
                        {
                            PaymentID = res.PaymentID,
                            VoucherCode = i.VoucherCode,
                        });
                    }
                }
                else
                {
                    return new MakeOperationModel()
                    {
                        ActionResultCode = res.ActionResultCode,
                        ActionResultMsg = res.ActionResultMsg,
                    };
                }
                if (details.ActionResultCode == 200)
                {
                    confirm = await _writeRepository.ConfirmOperation(new ConfirmOperationCommand()
                    {
                        PaymentID = res.PaymentID,
                    });
                    return new MakeOperationModel()
                    {
                        ActionResultCode = confirm.ActionResultCode,
                        ActionResultMsg = confirm.ActionResultMsg,
                        PaymentID = res.PaymentID,
                    };
                }
                else
                {
                    return new MakeOperationModel()
                    {
                        ActionResultCode = details.ActionResultCode,
                        ActionResultMsg = details.ActionResultMsg,
                    };
                }
            }
            catch (Exception ex)
            {
                return new MakeOperationModel()
                {
                    ActionResultCode = 500,
                    ActionResultMsg = ex.Message,
                };
            }

        }
    }
}
