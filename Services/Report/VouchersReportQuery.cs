using MediatR;
using System;

namespace WbApp.Services.Report
{
    public class VouchersReportQuery:IRequest<VouchersReportModel>
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Lang { get; set; }
    }
}
