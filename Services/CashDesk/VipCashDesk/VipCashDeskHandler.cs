using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WbApp.Interfaces;

namespace WbApp.Services.CashDesk.VipCashDesk
{
    public class VipCashDeskHandler : IRequestHandler<VipCashDeskCommand, VipCashDeskModel>
    {
        private readonly IWbReadOnlyRepository _readRepository;
        public VipCashDeskHandler(IWbReadOnlyRepository readRepository)
        {
            _readRepository = readRepository;
        }
        public async Task<VipCashDeskModel> Handle(VipCashDeskCommand request, CancellationToken cancellationToken)
        {
            var res = await _readRepository.GetVipCashDesk(request);
            return res;
        }
    }
}
