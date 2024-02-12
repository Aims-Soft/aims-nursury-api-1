using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace posCoreModuleApi.Entities
{
    public class ProductFromExcel
    {
        public string excelBase64 { get; set; }
        public int companyID { get; set; }
        public int businessID { get; set; }
        public int branchID { get; set; }
        public int moduleId { get; set; }
        public int userID { get; set; }
    }
}