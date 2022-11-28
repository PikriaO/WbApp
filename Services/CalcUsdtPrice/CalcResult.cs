using MediatR;

namespace WbApp.Services.CalcUsdtPrice
{
    public class CalcResult
    {
        public decimal? Usdt { get; set; }
        public decimal? Amount { get; set; }
        public decimal? FeePersent { get; set; }
        public decimal? FeeAmount { get; set; }
        public decimal? ToTalAmount { get; set; }
        public decimal? Rate { get; set; }
        public string ActionResultMsg { get; set; }
        public int ActionResultCode { get; set; }

    }
}
