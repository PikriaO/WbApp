using MediatR;

namespace WbApp.Services.GetUser
{
    public class GetUserCommand:IRequest<GetUserModel>
    {
        public int IsLegal { get; set; }
        public string Lang { get; set; }
        public string Inn { get; set; }
        public string PersonalID { get; set; }

    }
}
