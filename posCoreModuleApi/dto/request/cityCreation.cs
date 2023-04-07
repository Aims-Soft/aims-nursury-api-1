using System;
using System.Text.Json.Serialization;


namespace posCoreModuleApi.Entities
{
    public class CityCreation
    {
        public int cityID { get; set; }
        public string cityName { get; set; }
        public int userID { get; set; }
    }
}