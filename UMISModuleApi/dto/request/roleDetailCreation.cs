using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UMISModuleApi.Entities
{
    public class RoleDetailCreation
    {
        public int roleDetailID { get; set; }
        public int menuId { get; set; }
        public bool read { get; set; }
        public bool write { get; set; }
        public bool delete { get; set; }
    }
}