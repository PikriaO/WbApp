using MediatR;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WbApp.Interfaces;
using WbApp.Services.GetPaymentDetails;
using static WbApp.Services.CashDesk.FillCloseTransactions.TransactionsModel;

namespace WbApp.Services.Report
{
    public class VouchersReportHandler : IRequestHandler<VouchersReportQuery, VouchersReportModel>
    {
        private readonly IWbReadOnlyRepository _readRepository;

        public VouchersReportHandler(IWbReadOnlyRepository readRepository)
        {
            _readRepository = readRepository;

        }
        public async Task<VouchersReportModel> Handle(VouchersReportQuery request, CancellationToken cancellationToken)
        {
            var data = new VouchersReportModel();
            var datalist = new List<VouchersReportModel.VReport>();
            request.StartDate = request.StartDate == null ? DateTime.Now.AddMonths(-1):request.StartDate;
            request.EndDate = request.EndDate == null ? DateTime.Now : request.EndDate;
            data.StartDate = request.StartDate?.ToString("yyyy-MM-dd");
            data.EndDate = request.EndDate?.ToString("yyyy-MM-dd");
            int[] branches ={21, 22, 23};
            foreach (var i in branches)
            {
                var res = await _readRepository.GetCashDeskBalance(new CashDeskTransactions.CashDeskTransactionsQuery()
                {
                    BranchID =i,
                     Lang=request.Lang,
                    EndDate = request.EndDate,
                    StartDate = request.StartDate
                });
                var list = new VouchersReportModel.VReport
                { 
                    SAmount = $"{res.SumAmount:C}".Replace("$", ""),
                    SumAmount = res.SumAmount, 
                    BranchName = res.BranchName,
                    Count = res.CardCount, 
            };
                datalist.Add(list);
            }
            data.vReports = datalist;
            return data;
        }
    }
}
