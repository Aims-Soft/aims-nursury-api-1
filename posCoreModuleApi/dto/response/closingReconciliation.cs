using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace posCoreModuleApi.Entities
{
    public class ClosingReconciliation
    {
        public int shiftID { get; set; }
        public string shiftDate { get; set; }
        public float openingBalance { get; set; }
        public float closingBalance { get; set; }
        public float totalInflow { get; set; }
        public float totalOutflow { get; set; }
    }
}