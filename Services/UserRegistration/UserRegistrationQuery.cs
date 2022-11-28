using MediatR;
using System;

namespace WbApp.Services.UserRegistration
{
    public class UserRegistrationQuery : IRequest<UserRegistrationModel>
    {
        public string Lang { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string PersonalID { get; set; }
        public string CompanyName { get; set; }
        public string Inn { get; set; }
        public DateTime? BDate { get; set; }
        public int IsLegal { get; set; }
        public string Address { get; set; }
        public string Phone1 { get; set; }
        public string Description { get; set; }




    }
}
