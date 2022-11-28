using Org.BouncyCastle.Bcpg;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using static WbApp.Services.BuyNewUsdt.GetOperations.GetOperationsModel;

namespace WbApp.Services.GetUser
{
    public class GetUserModel
    {
        public int IsLegal { get; set; }
        public string ActionResultMsg { get; set; }
        public int ActionresultCode { get; set; }
        public int BranchName { get; set; }
        public IEnumerable<User> users { get; set; }
        public IEnumerable<Transaction> transactions { get; set; }
        public class User
        {
            public int ClientID { get; set; }
            public string PersonalCode { get; set; }
            public DateTime? BirthDate { get; set; }
            public string BDate { get; set; }
            public string Name { get; set; }
            public string Surname { get; set; }
            public string Phone { get; set; }
            public int IsLegal { get; set; }
            public string Inn { get; set; }
            public string Address { get; set; }
            public string CompanyName { get; set; }
            public string Email { get; set; }
            public string Phone1 { get; set; }
            public string AdditionalInfo { get; set; }
            public int? CategoryID { get; set; }
        }
    
    }
}
