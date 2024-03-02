using System;
using System.Text.Json.Serialization;


namespace posCoreModuleApi.Entities
{
    public class TotalAmount
    {
        public int appCounterInfoID { get; set; }
        public float totalAmount { get; set; }
    }
}