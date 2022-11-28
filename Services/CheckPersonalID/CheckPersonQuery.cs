using MediatR;

namespace WbApp.Services.CheckPersonalID
{
    public class CheckPersonQuery:IRequest<PersonDataModel>
    {
        public string PersonalID { get; set; }
        public string Inn { get; set; }
        public string Lang { get; set; }
    }
}
