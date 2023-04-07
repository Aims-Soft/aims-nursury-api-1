using System;
using System.Text.Json.Serialization;


namespace posCoreModuleApi.Entities
{
    public class EmployeeCreation
    {
        public int partyID { get; set; }
        public int designationID { get; set; }
        public int branchID { get; set; }
        public int cityID { get; set; }
        public string partyName { get; set; }
        public string partyNameUrdu { get; set; }
        public string address { get; set; }
        public string addressUrdu { get; set; }
        public string mobile { get; set; }
        // public string email { get; set; }
        public string type { get; set; }
        public string description { get; set; }
        public string cnic { get; set; }
        public int userID { get; set; }
        public int businessid { get; set; }
        public int companyid { get; set; }
    }
}