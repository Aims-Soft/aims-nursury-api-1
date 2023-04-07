using System;
using System.Text.Json.Serialization;


namespace posCoreModuleApi.Entities
{
    public class MonthlySalesDashboard
    {
        public string fld_monthname { get; set; }
        public float fld_amount { get; set; }
    }
}