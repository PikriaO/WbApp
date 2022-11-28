using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WbApp.Models;

namespace WbApp.Services.Authorisation
{
    public class AuthorisationQuery : IRequest<ActionResultModel>
    {
        public string Lang { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string BranchName { get; set; }
        public int BranchID { get; set; }
    }
}
