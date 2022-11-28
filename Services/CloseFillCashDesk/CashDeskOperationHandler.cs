using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WbApp.Interfaces;

namespace WbApp.Services.CloseFillCashDesk
{
    public class CashDeskOperationHandler : IRequestHandler<CashDeskOperationCommand, CashDeskOperationModel>
    {
        private readonly IWbWriteOnlyRepository _writeRepository;
        public CashDeskOperationHandler(IWbWriteOnlyRepository writeRepository)
        { 
            _writeRepository = writeRepository;
        }
        public async Task<CashDeskOperationModel> Handle(CashDeskOperationCommand request, CancellationToken cancellationToken)
        {

            var res = await _writeRepository.CloseFillCashDesk(request);

            return res;
        }
    }
}
