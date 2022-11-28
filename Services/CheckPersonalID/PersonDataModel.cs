using System;

namespace WbApp.Services.CheckPersonalID
{
    public class PersonDataModel
    {
        public string name { get; set; }
        public string surname { get; set; }
        public string Phone { get; set; }
        public DateTime? BDate { get; set; }
        public string BirthDate { get; set; }
        public string Inn { get; set; }
        public string phone1 { get; set; }
        public string OrgName { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string ActionResultMsg { get; set; }
        public int ActionResultCode { get; set; }
        public string AdditionalInfo { get; set; }
    }
}
