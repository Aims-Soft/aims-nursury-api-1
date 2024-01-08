using System;
using System.Text.Json.Serialization;


namespace reportApi.Entities
{
    public class StockInStockOut
    {
        public int productID { get; set; }
        public string productName { get; set; }
        public int stockin { get; set; }
        public int stockout { get; set; }
        public int remainingstock { get; set; }
        public float costPrice { get; set; }
        public float remainingcostprice { get; set; }
    }
}