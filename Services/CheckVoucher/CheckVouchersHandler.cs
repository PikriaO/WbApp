using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WbApp.Interfaces;

namespace WbApp.Services.CheckVoucher
{
    public class CheckVouchersHandler : IRequestHandler<CheckVoucherQuery, CheckVoucherModel>
    {
        private readonly IWbReadOnlyRepository _readRepository;
        public CheckVouchersHandler(IWbReadOnlyRepository readRepository)
        {
            _readRepository = readRepository;
        }
        public async Task<CheckVoucherModel> Handle(CheckVoucherQuery request, CancellationToken cancellationToken)
        {
            var res = new CheckVoucherModel();
            res = await _readRepository.CheckVoucher(request);
            return res; 

        }
    }
}
