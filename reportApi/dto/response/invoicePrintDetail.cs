using System;
using System.Text.Json.Serialization;


namespace reportApi.Entities
{
    public class InvoicePrintDetail
    {
        public int invoiceNo { get; set; }
        public string invoiceDate { get; set; }
        public string partyname { get; set; }
        public int discount { get; set; }
        public string productName { get; set; }
        public int qty { get; set; }
        public int change { get; set; }
        public int salePrice { get; set; }
        public int totalsalePrice { get; set; }
        public int cashReceived { get; set; }
        public int bankcashReceived { get; set; }
        public int grandTotal { get; set; }
        public int branchid { get; set; }
        public int companyid { get; set; }
        public string mobileNo { get; set; }
        public string businessFullName { get; set; }
        public string businessShortName { get; set; }
        public string empName { get; set; }
        public int userID { get; set; }
        public int subtotal { get; set; }
    }
}