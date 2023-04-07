using System;
using System.Text.Json.Serialization;


namespace posCoreModuleApi.Entities
{
    public class SaleReturn
    {
        public int invoiceNo { get; set; }
        // public int partyID { get; set; }
        // public int invoiceDetailID { get; set; }
        public int productID { get; set; }
        public float qty { get; set; }
        // public float costPrice { get; set; }
        // public string invoiceDate { get; set; }
        // public string invoiceType { get; set; }
    }
}