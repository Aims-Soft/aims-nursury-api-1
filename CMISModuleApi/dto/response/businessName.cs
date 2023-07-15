using System;
using System.Text.Json.Serialization;


namespace CMISModuleApi.Entities
{
    public class BusinessName
    {
        public int branchID { get; set; }
        public string branchName { get; set; }
        public string email { get; set; }
        public string mobileNo { get; set; }
        public string businessFullName { get; set; }
        public string businessShortName { get; set; }
    }
}