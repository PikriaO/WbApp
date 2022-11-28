using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WbApp.Interfaces;
using WbApp.Services.CashDesk.VipCashDesk;

namespace WbApp.Services.BuyNewUsdt.GetOperations
{
    public class GetOperationsHandler : IRequestHandler<GetOperationsCommand, GetOperationsModel>
    {
        private readonly IWbReadOnlyRepository _readRepository;

        public GetOperationsHandler(IWbReadOnlyRepository readRepository)
        {
            _readRepository = readRepository;

        }
        public async Task<GetOperationsModel> Handle(GetOperationsCommand request, CancellationToken cancellationToken)
        {
            var data = await _readRepository.GetOperations(request);
            var vipcashdesk = await _readRepository.GetVipCashDesk(new VipCashDeskCommand
            {
                BranchID = request.BranchID,
                EndDate = request.EndDate,
                StartDate = request.StartDate,
                Lang = request.Lang
            });
            data.viptran = vipcashdesk.vipTrans;

            data.ToTalAmount = vipcashdesk.ToTalAmount;
            data.CToTalAmount = $"{data.ToTalAmount:C}".Replace("$", "").Replace("¤", "");
            data.Amount = vipcashdesk.Amount;
            data.CAmount = $"{data.Amount:C}".Replace("$", "").Replace("¤", "");
            data.CFeeAmount = $"{data.FeeAmount:C}".Replace("$", "").Replace("¤", "");
            data.AccountNumber = vipcashdesk.AccountNumber;
            data.TrAccount = vipcashdesk.TrAccount;
            return data;
        }
    }
}
