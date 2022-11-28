using System.Collections;
using System.Collections.Generic;

namespace WbApp.Services.Branches
{
    public class GetBranchesModel
    {
        public int ActionResultCode { get; set; }
        public string ActionResultMsg { get; set; }
        public IEnumerable<Branch> branches { get; set; }

        public class Branch
        {
            public int? Id { get; set; }
            public string Name { get; set; }
            public string Address { get; set; }

        }
    }
}
