using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WbApp.Interfaces;

namespace WbApp.Services.BuyNewUsdt.ConfirmUsdtCashIn
{
    public class ConfirmCashInHandler : IRequestHandler<ConfirmCashInCommand, ConfirmCashInModel>
    {
        private readonly IWbReadOnlyRepository _readRepository;

        public ConfirmCashInHandler(IWbReadOnlyRepository readRepository)
        {
            _readRepository = readRepository;
        }
        public async Task<ConfirmCashInModel> Handle(ConfirmCashInCommand request, CancellationToken cancellationToken)
        {
            var res = await _readRepository.ConfirmUsdtCashIn(request);
            return res;
        }
    }
}
