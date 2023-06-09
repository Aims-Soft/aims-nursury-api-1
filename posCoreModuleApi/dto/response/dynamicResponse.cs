using System;
using System.Text.Json.Serialization;


namespace posCoreModuleApi.Entities
{
    public class dynamicResponse
    {
        public string instanceName { get; set; }
        public string userName { get; set; }
        public string credentials { get; set; }
        public string dbName { get; set; }
    }
}