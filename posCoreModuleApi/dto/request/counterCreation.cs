using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace posCoreModuleApi.Entities
{
    public class CounterCreation
    {
        public int counterID { get; set; }
        public string counterName { get; set; }
        public int counterNo { get; set; }
        public float grassAmount { get; set; }
        public int businessid { get; set; }
        public int companyid { get; set; }
        public int userID { get; set; }
        public string spType { get; set; }
    }
}