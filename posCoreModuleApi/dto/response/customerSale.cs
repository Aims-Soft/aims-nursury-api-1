using System;
using System.Text.Json.Serialization;


namespace posCoreModuleApi.Entities
{
    public class CustomerSale
    {
        public int partyID { get; set; }
       
        public string partyName { get; set; }
      
        public string mobile { get; set; }
       
        public string type { get; set; }
       
        public string cnic { get; set; }
      
        public int isDeleted { get; set; }
       
    }
}