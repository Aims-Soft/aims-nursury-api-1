using System;
using System.Text.Json.Serialization;


namespace posCoreModuleApi.Entities
{
    public class DailySalesDashboard
    {
        public string fld_monthday { get; set; }
        public float fld_amount { get; set; }
        
    }
}