using System;
using System.Text.Json.Serialization;


namespace posCoreModuleApi.Entities
{
    public class Route
    {
        public int rootID { get; set; }
        public string rootName { get; set; }
        public int isDeleted { get; set; }
        public int businessid { get; set; }
        public int companyid { get; set; }
    }
}