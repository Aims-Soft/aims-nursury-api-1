using System;
using System.Text.Json.Serialization;


namespace posCoreModuleApi.Entities
{
    public class InvoiceDetailCreation
    {
        public int invoiceDetailID { get; set; }
        public int invoiceNo { get; set; }
        public int locationID { get; set; }
        public int productID { get; set; }
        public int qty { get; set; }
        public Nullable<int> schemeQty { get; set; }
        public float costPrice { get; set; }
        public float salePrice { get; set; }
        public float discount { get; set; }
        public float gst { get; set; }
        public float et { get; set; }
        public string expiryDate { get; set; }
        public string mfgDate { get; set; }
        public string batchNo { get; set; }
        public string batchStatus { get; set; }
        public string schemeType { get; set; }
        public string schemeStatus { get; set; }
        public Nullable<int> productQty { get; set; }
        public int schemeLimit { get; set; }
        public string productName { get; set; }
        public string pctCode { get; set; }
    }
}