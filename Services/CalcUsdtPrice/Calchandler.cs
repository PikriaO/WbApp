using MediatR;
using Microsoft.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using WbApp.Interfaces;

namespace WbApp.Services.CalcUsdtPrice
{
    public class Calchandler : IRequestHandler<CalcCommand, CalcResult>
    {
        private readonly IWbReadOnlyRepository _readRepository;

        public Calchandler(IWbReadOnlyRepository readRepository)
        {
            _readRepository = readRepository;
        }
        public async Task<CalcResult> Handle(CalcCommand request, CancellationToken cancellationToken)
        {
            var data = await _readRepository.CalcPrice(request);

            return data;
        }
    }
}


