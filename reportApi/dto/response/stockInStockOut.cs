using System;
using System.Text.Json.Serialization;


namespace reportApi.Entities
{
    public class StockInStockOut
    {
        public int productID { get; set; }
        public string productName { get; set; }
        public int stockin { get; set; }
    }
}