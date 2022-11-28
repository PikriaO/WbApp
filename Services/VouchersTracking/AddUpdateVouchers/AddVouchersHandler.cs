using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WbApp.Interfaces;

namespace WbApp.Services.VouchersTracking.AddUpdateVouchers
{
    public class AddVouchersHandler : IRequestHandler<AddVouchersCommand, AddVouchersModel>
    {
        private readonly IWbWriteOnlyRepository _writeRepository;

        public AddVouchersHandler(IWbWriteOnlyRepository writeRepository)
        {
            _writeRepository = writeRepository;
        }
        public async Task<AddVouchersModel> Handle(AddVouchersCommand request, CancellationToken cancellationToken)
        {
            var res = new AddVouchersModel();

            foreach (var i in request.vouchersData)
            {
                res = await _writeRepository.AddNewVouchers(new VouchersList()
                {
                    Amount = i.Amount,
                    Lang = i.Lang,
                    BranchID = i.BranchID,
                    Code = i.Code,
                    FileName = i.FileName,
                    Mode = i.Mode,
                    Usdt = i.Usdt,
                });
            }
            return res;

        }
    }
}
