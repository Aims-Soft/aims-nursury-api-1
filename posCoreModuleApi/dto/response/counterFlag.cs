using System;
using System.Text.Json.Serialization;


namespace posCoreModuleApi.Entities
{
    public class CounterFlag
    {
        public int counterFlagID { get; set; }
        public string counterFlagTitle { get; set; }
        public bool counterFlag { get; set; }
    }
}