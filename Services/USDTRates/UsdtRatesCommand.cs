using MediatR;
using System;
using System.Collections.Generic;

namespace WbApp.Services.USDTRates
{
    public class UsdtRatesCommand : IRequest<UsdtRatesModel>
    {
        public string Lang { get; set; }
        public int? Mode { get; set; }
        public int? Usdt { get; set; }
        public decimal? Price { get; set; }
        public DateTime? Dt { get; set; }
        public int ActionType { get; set; } //1 getlist //2update //insert
        public List<Usdtarray> usdtarray { get; set; }

    }
    public class Usdtarray
    {

        public int Usdt { get; set; }
        public decimal Price { get; set; }
        public DateTime? Dt { get; set; }
        public DateTime? CreateDate { get; set; }
        public string Lang { get; set; }
    }
}