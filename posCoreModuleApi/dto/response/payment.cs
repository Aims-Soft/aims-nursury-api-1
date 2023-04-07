using System;
using System.Text.Json.Serialization;


namespace posCoreModuleApi.Entities
{
    public class Payment
    {
        public int invoiceNo { get; set; }
        public int partyID { get; set; }
        public string invoiceDate { get; set; }
        public float cashReceived { get; set; }
        public float discount { get; set; }
        public string invoiceType { get; set; }
        public string partyName { get; set; }
        public string description { get; set; }
        public int businessid { get; set; }
        public int companyid { get; set; }
        public int branchid { get; set; }
    }
}