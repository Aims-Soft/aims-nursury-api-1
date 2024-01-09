using System;
using System.Text.Json.Serialization;


namespace reportApi.Entities
{
    public class InvoiceDetail
    {
        public int invoiceNo { get; set; }
        public string invoiceType { get; set; }
        public string invoiceDate { get; set; }
        public int branchid { get; set; }
        public int companyid { get; set; }
        public float amount { get; set; }
        public float totalamount { get; set; }
        public string partyname { get; set; }
    }
}