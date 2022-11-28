using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WbApp.Interfaces;
using WbApp.Services.BuyNewUsdt.GetOperations;

namespace WbApp.Services.GetUser
{
    public class GetUserHandler : IRequestHandler<GetUserCommand, GetUserModel>
    {
        private readonly IWbReadOnlyRepository _readRepository;

        public GetUserHandler(IWbReadOnlyRepository readRepository)
        {
            _readRepository = readRepository;
        }
        public async Task<GetUserModel> Handle(GetUserCommand request, CancellationToken cancellationToken)
        {
            var useroperations = await _readRepository.GetOperations(new GetOperationsCommand
            {
                Inn = request.Inn,
                Lang = request.Lang,
                PersonalID = request.PersonalID,
            });
            var data = await _readRepository.GetUser(request);
            data.transactions = useroperations.transactions;
            return data;
        }
    }
}
