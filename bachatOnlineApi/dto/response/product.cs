using System;
using System.Text.Json.Serialization;


namespace bachatOnlineModuleApi.Entities
{
    public class Product
    {
        public int productID { get; set; }
        public string productName { get; set; }
        public float salePrice { get; set; }
        public string applicationedoc { get; set; }
        public string categoryName { get; set; }
        public int invoiceNo { get; set; }
        public int invoiceDetailID { get; set; }
        public int qty { get; set; }
        public int availQty { get; set; }
        public int branchid { get; set; }
        public int businessid { get; set; }
        public int companyid { get; set; }
    }
}