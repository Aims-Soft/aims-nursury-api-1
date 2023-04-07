namespace UMISModuleAPI.Entities
{
    public class Menu
    {
        public int applicationModuleId { get; set; }
        public string applicationModuleTitle { get; set; }
        public string moduleIcon { get; set; }
        public int roleId { get; set; }
        public string roleTitle { get; set; }
        public int menuId { get; set; }
        public int parentMenuId { get; set; }
        public float menuSeq { get; set; }
        public string menuTitle { get; set; }
        public string parentRoute { get; set; }
        public string routeTitle { get; set; }
        public bool read { get; set; }
        public bool write { get; set; }
        public bool delete { get; set; }
    }
}