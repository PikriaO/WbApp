using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WbApp.Interfaces;
using WbApp.Models;

namespace WbApp.Services.Branches
{
    public class GetBranchesHandler : IRequestHandler<GetBranchesCommand, ActionResultModel>
    {
        private readonly IWbReadOnlyRepository _readRepository;

        public GetBranchesHandler(IWbReadOnlyRepository readRepository)
        {
            _readRepository = readRepository;
        }
        public async Task<ActionResultModel> Handle(GetBranchesCommand request, CancellationToken cancellationToken)
        {
            var res = await _readRepository.GetBranches(request);
            return res;
        }
    }
}
