using System;
using System.Text.Json.Serialization;


namespace bachatOnlineModuleApi.Entities
{
    public class OrderDetailCreation
    {
        public int orderDetailID { get; set; }
        public int orderID { get; set; }
        public int productID { get; set; }
        public int invoiceNo { get; set; }
        public int invoiceDetailID { get; set; }
        public string productName { get; set; }
        public int qty { get; set; }
        public int availQty { get; set; }
        public float salePrice { get; set; }
        
    }
}