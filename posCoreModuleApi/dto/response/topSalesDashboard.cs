using System;
using System.Text.Json.Serialization;


namespace posCoreModuleApi.Entities
{
    public class TopSalesDashboard
    {
        public string productName { get; set; }
        public float salePrice { get; set; }
    }
}