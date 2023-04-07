using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CMISModuleApi.Entities
{
    public class BranchCreation
    {
        public int branchID { get; set; }
        public int businessBranchID { get; set; }
        public int businessID { get; set; }
        public string branchName { get; set; }
        public string branchAddress { get; set; }
        public string email { get; set; }
        public string mobileNo { get; set; }
        public string phoneNo { get; set; }
        public int userID { get; set; }
    }
}