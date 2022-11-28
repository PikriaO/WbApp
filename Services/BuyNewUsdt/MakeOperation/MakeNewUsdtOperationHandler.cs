using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WbApp.Interfaces;

namespace WbApp.Services.BuyNewUsdt.MakeOperation
{
    public class MakeNewUsdtOperationHandler : IRequestHandler<MakeNewUsdtOperationQuery, MakeNewUsdtOperationModel>
    {

        private readonly IWbWriteOnlyRepository _writeRepository;


        public MakeNewUsdtOperationHandler(IWbWriteOnlyRepository writeRepository)
        {
            _writeRepository = writeRepository;
        }
        public async Task<MakeNewUsdtOperationModel> Handle(MakeNewUsdtOperationQuery request, CancellationToken cancellationToken)
        {
            var res = await _writeRepository.BuyNewUsdt(request);
            return res;
        }
    }
}
