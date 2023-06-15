using System;
using System.Text.Json.Serialization;


namespace posCoreModuleApi.Entities
{
    public class CustomerCreation
    {
        public int partyID { get; set; }
        public int moduleId { get; set; }
        public int businessid { get; set; }
        public int companyid { get; set; }
        public int branchID { get; set; }
        public string partyName { get; set; }
        public string mobile { get; set; }
        public string type { get; set; }
        public string cnic { get; set; }
        public int userID { get; set; }
    }
}