namespace UMISModuleAPI.Entities
{
    public class RoleOption
    {
        public int applicationModuleId { get; set; }
        public int menuId { get; set; }
        public int roleId { get; set; }
        public string applicationModuleTitle { get; set; }
        public string menuTitle { get; set; }
        public bool read { get; set; }
        public bool write { get; set; }
        public bool delete { get; set; }
    }
}