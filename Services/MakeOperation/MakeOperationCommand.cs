using MediatR;
using System;
using System.Collections.Generic;

namespace WbApp.Services.MakeOperation
{
    public class MakeOperationCommand:IRequest<MakeOperationModel>
    {
        public int OperatorID { get; set; }
        public int BranchID { get; set; }
        public int CardCount { get; set; }
        public int Mode { get; set; }
        public decimal? TerminalAmount { get; set; }
        public decimal? BankAmount { get; set; }
        public decimal? CashAmount { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string PersonalID { get; set; }
        public string Phone { get; set; }
        public DateTime BirthDate { get; set; }
        public string Lang { get; set; }
        public List<Vouchers>  vouchers { get; set; }

    }
    public class Vouchers
    {
        
        public string VoucherCode { get; set; }

    }
}
