using System;
using System.Text.Json.Serialization;


namespace posCoreModuleApi.Entities
{
    public class Location
    {
        public int locationID { get; set; }
        public string locationName { get; set; }
        public string description { get; set; }
        public int parentLocationID { get; set; }
        public int isDeleted { get; set; }
        public int businessid { get; set; }
        public int companyid { get; set; }
    }
}