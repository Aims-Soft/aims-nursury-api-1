using System;
using System.Text.Json.Serialization;


namespace posCoreModuleApi.Entities
{
    public class Invoice
    {
        public int invoiceNo { get; set; }
        public string invoiceDate { get; set; }
        public int partyID { get; set; }
        public int bankID { get; set; }
        public int refInvoiceNo { get; set; }
        public string refInvoiceDate { get; set; }
        public Nullable<float> cashReceived { get; set; }
        public float discount { get; set; }
        public float change { get; set; }
        public string fbrInvoiceNo { get; set; }
        public string fbrCode { get; set; }
        public string fbrResponse { get; set; }
        public int approvedBy { get; set; }
        public string approvedOn { get; set; }
        public string creditCardNo { get; set; }
        public string description { get; set; }
        public Nullable<int> bookID { get; set; }
        public Nullable<int> branchID { get; set; }
        public int userID { get; set; }
        public string json { get; set; }
        public int businessid { get; set; }
        public int companyid { get; set; }
        public int branchid { get; set; }
    }
}