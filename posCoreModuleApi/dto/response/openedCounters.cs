using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace posCoreModuleApi.Entities
{
    public class OpenedCounters
    {
        public int counterID { get; set; }
        public string counterName { get; set; }
        public int counterNo { get; set; }
        public int shiftID { get; set; }
        public int managerID { get; set; }
        public string userName { get; set; }
        public int userID { get; set; }
        public string empName { get; set; }
        public string openingTime { get; set; }
        public string closingTime { get; set; }
        public string shiftDate { get; set; }
        public float openingBalance { get; set; }
        public string status { get; set; }
        public int companyID { get; set; }
        public int businessID { get; set; }
        public int branchID { get; set; }
    }
}