using System;
using System.Text.Json.Serialization;


namespace posCoreModuleApi.Entities
{
    public class OpeningBalance
    {
        public string invoiceDate { get; set; }
        public int invoiceNo { get; set; }
        public int productID { get; set; }
        public float costPrice { get; set; }
        public float qty { get; set; }
        public string productName { get; set; }
        public string barcode1 { get; set; }
        public int businessid { get; set; }
        public int companyid { get; set; }
        public int branchid { get; set; }
    }
}