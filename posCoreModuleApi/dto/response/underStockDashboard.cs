using System;
using System.Text.Json.Serialization;


namespace posCoreModuleApi.Entities
{
    public class UnderStockDashboard
    {
        public string productname { get; set; }
        public float totalqty { get; set; }
    }
}