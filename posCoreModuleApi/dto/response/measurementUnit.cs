using System;
using System.Text.Json.Serialization;


namespace posCoreModuleApi.Entities
{
    public class MeasurementUnit
    {
        public int uomID { get; set; }
        public string uomName { get; set; }
        public int isDeleted { get; set; }
    }
}