using System;
using System.Text.Json.Serialization;


namespace posCoreModuleApi.Entities
{
    public class Currency
    {
        public int currencyID { get; set; }
        public string currencyTitle { get; set; }
        public float denomination { get; set; }
    }
}