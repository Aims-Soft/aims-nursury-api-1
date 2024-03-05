using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace posCoreModuleApi.Entities
{
    public class PackageDetail
    {
        public int packageID { get; set; }
        public string packageTitle { get; set; }
        public string barcode { get; set; }
        public string packageDate { get; set; }
        public int businessID { get; set; }
        public int companyID { get; set; }
        public int branchID { get; set; }
        public int productID { get; set; }
        public string productName { get; set; }
        public string productNameUrdu { get; set; }
        public float salePrice { get; set; }
        public string productBarcode { get; set; }
    }
}