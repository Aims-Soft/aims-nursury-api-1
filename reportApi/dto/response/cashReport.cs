using System;
using System.Text.Json.Serialization;


namespace reportApi.Entities
{
    public class CashReport
    {
        public int coaID { get; set; }
        public string invoiceDate { get; set; }
        public int partyID { get; set; }
        public string partyName { get; set; }
        public string invoiceType { get; set; }
        public int invoiceNo { get; set; }
        public int branchid { get; set; }
        public int businessid { get; set; }
        public int companyid { get; set; }
        public float debit { get; set; }
        public float credit { get; set; }
    }
}