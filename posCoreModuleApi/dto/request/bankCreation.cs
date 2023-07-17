using System;
using System.Text.Json.Serialization;


namespace posCoreModuleApi.Entities
{
    public class BankCreation
    {
        public int bankID { get; set; }
        public int moduleId { get; set; }
        public int coaID { get; set; }
        public string bankName { get; set; }
        public string branchCode { get; set; }
        public string branchAddress { get; set; }
        public string accountNo { get; set; }
        public string accountTitle { get; set; }
        public string description { get; set; }
        public string type { get; set; }
        public string branchname { get; set; }
        public float amount { get; set; }
        public int userID { get; set; }
        public int businessid { get; set; }
        public int companyid { get; set; }
        public int branchID { get; set; }

    }
}