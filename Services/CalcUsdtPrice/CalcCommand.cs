using MediatR;

namespace WbApp.Services.CalcUsdtPrice
{
    public class CalcCommand : IRequest<CalcResult>
    {
        public decimal? usdt { get; set; }
        public decimal? Amount { get; set; }
        public int? Category { get; set; }
        public string Lang { get; set; }
    }
}
