using System;
using System.Text.Json.Serialization;


namespace posCoreModuleApi.Entities
{
    public class CompanyName
    {
        public string instanceName { get; set; }
        public string dbName { get; set; }
        public string userName { get; set; }
        public string credentails { get; set; }
    }
}