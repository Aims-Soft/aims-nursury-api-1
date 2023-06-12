using System;
using System.Text.Json.Serialization;


namespace posCoreModuleApi.Entities
{
    public class PaymentCreation
    {
        public int invoiceNo { get; set; }
        public int moduleId { get; set; }
        public int invoiceDetailID { get; set; }
        public int partyID { get; set; }
        public int categoryID { get; set; }
        public int coaID { get; set; }
        public string type { get; set; }
        public string invoiceDate { get; set; }
        public float amount { get; set; }
        public float discount { get; set; }
        public string description { get; set; }
        public int userID { get; set; }
        public int businessid { get; set; }
        public int companyid { get; set; }
        public int branchid { get; set; }
    }
}