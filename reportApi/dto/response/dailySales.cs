using System;
using System.Text.Json.Serialization;


namespace reportApi.Entities
{
    public class DailySales
    {
        public string invoiceNo { get; set; }
        public string invoiceDate { get; set; }
        public string productName { get; set; }
        public int qty { get; set; }
        public int costPrice { get; set; }
        public int salePrice { get; set; }
        public int margin { get; set; }
        public string invoiceType { get; set; }
    }
}