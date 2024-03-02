using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace posCoreModuleApi.Entities
{
    public class Package
    {
        public int packageDetailID { get; set; }
        public int packageID { get; set; }
        public string packageTitle { get; set; }
        public string packageDate { get; set; }
        public int branchID { get; set; }
    }
}