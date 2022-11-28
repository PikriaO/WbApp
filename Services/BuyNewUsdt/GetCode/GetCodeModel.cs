using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;

namespace WbApp.Services.BuyNewUsdt.GetCode
{
    public class GetCodeModel
    {
        public string Code { get; set; }
        public string Base64 { get; set; }
        public decimal Usdt { get;set; }
        public DateTime CreateDate { get; set; }
        public string FormatedDate { get; set; }
        public string ActionResultMsg { get; set; }
        public int ActionResultCode { get; set; }
    }
}
