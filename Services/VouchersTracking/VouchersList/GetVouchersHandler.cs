using MediatR;
using Microsoft.Graph;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WbApp.Interfaces;

namespace WbApp.Services.VouchersTracking.VouchersList
{
    public class GetVouchersHandler : IRequestHandler<GetVouchersQuery, GetVouchersListModel>
    {
        private readonly IWbReadOnlyRepository _readRepository;

        public GetVouchersHandler(IWbReadOnlyRepository readRepository)
        {
            _readRepository = readRepository;

        }
        public async Task<GetVouchersListModel> Handle(GetVouchersQuery request, CancellationToken cancellationToken)
        {
            var res = new GetVouchersListModel();
            var t = new List<GetVouchersListModel.VoucherFile>();
            var data = await _readRepository.GetVouchersList(request);
            var DistinctItems = data.voucher.GroupBy(o => o.FIleName).Select(i => i.First()).OrderByDescending(u => u.CreateDate);
            foreach (var i in DistinctItems)
            {
                GetVouchersListModel.VoucherFile a = new GetVouchersListModel.VoucherFile();
                a.FIleName = i.FIleName;
                a.CreateDate = i.CreateDate;
                a.Count = data.voucher.Where(i => i.FIleName == a.FIleName).Count();
                t.Add(a);
            }
            return new GetVouchersListModel()
            {
                voucherFiles = t,
                voucher = data.voucher,
            };

        }
    }
}
