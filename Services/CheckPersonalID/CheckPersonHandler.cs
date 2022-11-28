using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WbApp.Interfaces;

namespace WbApp.Services.CheckPersonalID
{
    public class CheckPersonHandler : IRequestHandler<CheckPersonQuery, PersonDataModel>
    {
        private readonly IWbReadOnlyRepository _readRepository;
        public CheckPersonHandler(IWbReadOnlyRepository readRepository)
        {
            _readRepository = readRepository;
        }
        public async Task<PersonDataModel> Handle(CheckPersonQuery request, CancellationToken cancellationToken)
        {
            var data = await _readRepository.CheckPersonalID(request);

            return data;
        }
    }
}
