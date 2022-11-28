using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WbApp.Models
{
   public class ActionResultModel
    {
        public string ActionResultMsg { get; set; }
        public int ActionResultCode { get; set; }
        public string ErrorMessage { get; set; }
        public string UserName { get; set; }
        public string BranchName { get; set; }
        public string OperatorName { get; set; }
        public int BranchID { get; set; }
        public int UserID { get; set; }
        public int RoleID { get; set; }

        public IEnumerable<Branch> branches { get; set; }

        public class Branch
        {
            public int? Id { get; set; }
            public string Name { get; set; }
            public string Address { get; set; }

        }
    }
}
