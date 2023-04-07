using System;
using System.Text.Json.Serialization;


namespace posCoreModuleApi.Entities
{
    public class City
    {
        public int cityID { get; set; }
        public string cityName { get; set; }
        public int isDeleted { get; set; }
    }
}