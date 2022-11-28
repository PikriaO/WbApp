using MediatR;
using System;

namespace WbApp.Services.BuyNewUsdt.GetOperations
{
    public class GetOperationsCommand:IRequest<GetOperationsModel>
    {
        public string PersonalID { get; set; }
        public string Inn { get; set; }
        public int? OperationID { get; set; }
        public string Lang { get; set; }
        public int? Status { get; set; }
        public int? BranchID { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
