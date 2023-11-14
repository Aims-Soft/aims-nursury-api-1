using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace posCoreModuleApi.Entities
{
    public class ImportProductCreation
    {
        public string json { get; set; }
        public int companyID { get; set; }
        public int businessID { get; set; }
        public int branchID { get; set; }
        public int moduleId { get; set; }
        public int userID { get; set; }
    }
}