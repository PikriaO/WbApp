using System.Collections.Generic;

namespace WbApp.Services.Report
{
    public class VouchersReportModel
    {
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public IEnumerable<VReport> vReports { get; set; }
        public class VReport
        {
            public string BranchName { get; set; }
            public decimal SumAmount { get; set; }
            public string SAmount { get; set; }
            public int Count { get; set; }
        }
    }
}
