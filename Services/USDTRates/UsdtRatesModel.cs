using System;
using System.Collections.Generic;

namespace WbApp.Services.USDTRates
{
    public class UsdtRatesModel
    {
        public int? Mode { get; set; }
        public string ActionResultMsg { get; set; }
        public int ActionResultCode { get; set; }
        public IEnumerable<CurrentRate> currentRates { get; set; }

        public class CurrentRate
        {
            public int Id { get; set; }
            public int? Usdt { get; set; }
            public int? RateID { get; set; }
            public DateTime? DT { get; set; }
            public string DtFormatted { get; set; }
            public string Currency { get; set; }
            public decimal? Price { get; set; }
            public DateTime? CreateDate { get; set; }
            public string FormatedDate { get; set; }

        }
    }
}
