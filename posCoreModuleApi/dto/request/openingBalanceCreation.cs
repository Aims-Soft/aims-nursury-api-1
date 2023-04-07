using System;
using System.Text.Json.Serialization;


namespace posCoreModuleApi.Entities
{
    public class OpeningBalanceCreation
    {
        public string invoiceDate { get; set; }
        public int invoiceNo { get; set; }
        public int productID { get; set; }
        public int locationID { get; set; }
        public float costPrice { get; set; }
        public float salePrice { get; set; }
        public float qty { get; set; }
        public float debit { get; set; }
        public string productName { get; set; }
        public int userID { get; set; }
        public int businessid { get; set; }
        public int companyid { get; set; }
        public int branchid { get; set; }
    }
}