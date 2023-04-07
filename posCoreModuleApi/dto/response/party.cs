using System;
using System.Text.Json.Serialization;


namespace posCoreModuleApi.Entities
{
    public class Party
    {
        public int partyID { get; set; }
        public int cityID { get; set; }
        public int rootID { get; set; }
        public int branchID { get; set; }
        public int designationID { get; set; }
        public string desginationName { get; set; }
        public string partyName { get; set; }
        public string cityName { get; set; }
        public string rootName { get; set; }
        public string partyNameUrdu { get; set; }
        public string address { get; set; }
        public string addressUrdu { get; set; }
        public string phone { get; set; }
        public string mobile { get; set; }
        public string email { get; set; }
        public string type { get; set; }
        public string description { get; set; }
        public string cnic { get; set; }
        public string focalperson { get; set; }
        public string partyCode { get; set; }
        public string lcType { get; set; }
        public float lcAmount { get; set; }
        public int isDeleted { get; set; }
        public int businessid { get; set; }
        public int companyid { get; set; }
    }
}