using System;
using System.Text.Json.Serialization;


namespace reportApi.Entities
{
    public class dynamicResponse
    {
        public string instanceName { get; set; }
        public string userName { get; set; }
        public string credentials { get; set; }
        public string dbName { get; set; }
        
    }
}