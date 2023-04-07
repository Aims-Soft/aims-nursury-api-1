using System;
using System.Text.Json.Serialization;


namespace posCoreModuleApi.Entities
{
    public class OpeningBalanceProduct
    {
        public int productID { get; set; }
        public string productName { get; set; }
        public float costPrice { get; set; }
        public float salePrice { get; set; }
        public int qty { get; set; }
        public int invoiceNo { get; set; }
        public int branchid { get; set; }
        public int businessid { get; set; }
        public int companyid { get; set; }
    }
}