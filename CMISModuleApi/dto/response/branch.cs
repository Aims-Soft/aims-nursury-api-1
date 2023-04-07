using System;
using System.Text.Json.Serialization;


namespace CMISModuleApi.Entities
{
    public class Branch
    {
        public int branchID { get; set; }
        public string branchName { get; set; }
        public string branchAddress { get; set; }
        public string email { get; set; }
        public string mobileNo { get; set; }
        public string phoneNo { get; set; }
        public string businessFullName { get; set; }
        public int businessID { get; set; }
        public int businessBranchID { get; set; }
    }
}