namespace UMISModuleAPI.Entities
{
    public class Roles
    {
        public int roleId { get; set; }
        public int companyID { get; set; }
        public int busniessID { get; set; }
        public int branchID { get; set; }
        public string roleTitle { get; set; }
        public string roleDescription { get; set; }
        // public bool active { get; set; }
    }
}