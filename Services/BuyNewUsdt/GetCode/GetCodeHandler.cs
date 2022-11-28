using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WbApp.Interfaces;

namespace WbApp.Services.BuyNewUsdt.GetCode
{
    public class GetCodeHandler : IRequestHandler<GetCodeCommand, GetCodeModel>
    {
        private readonly IWbReadOnlyRepository _readRepository;

        public GetCodeHandler(IWbReadOnlyRepository readRepository)
        {
            _readRepository = readRepository;
        }
        public async Task<GetCodeModel> Handle(GetCodeCommand request, CancellationToken cancellationToken)
        {
            var res = await _readRepository.GetCode(request);
            var code = await _readRepository.GenerateQr(res.Code);
            res.Base64 = code;
            return res;
        }
    }
}
