using MediatR;
using WbApp.Models;

namespace WbApp.Services.Branches
{
    public class GetBranchesCommand:IRequest<ActionResultModel>
    {
        public string lang { get; set; }
    }
}
