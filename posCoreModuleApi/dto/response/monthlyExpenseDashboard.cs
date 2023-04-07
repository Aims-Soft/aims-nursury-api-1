using System;
using System.Text.Json.Serialization;


namespace posCoreModuleApi.Entities
{
    public class MonthlyExpenseDashboard
    {
        public string monthno { get; set; }
        public float income { get; set; }
        public float expense { get; set; }
    }
}