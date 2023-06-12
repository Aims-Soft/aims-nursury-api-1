using System;
using System.Text.Json.Serialization;


namespace posCoreModuleApi.Entities
{
    public class RouteCreation
    {
        public int rootID { get; set; }
        public int moduleId { get; set; }
        public string rootName { get; set; }
        public int userID { get; set; }
        public int businessid { get; set; }
        public int companyid { get; set; }
    }
}