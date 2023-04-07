using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UMISModuleApi.Entities
{
    public class UserAddress
    {
        public int userAddressID { get; set; }
        public int userID { get; set; }
        public string city { get; set; }
        public string address { get; set; }
        public string area { get; set; }
        public string label { get; set; }
    }
}