namespace UMISModuleAPI.Entities
{
    public class ApplicationMenu
    {
        public int applicationModuleId { get; set; }
        public int menuId { get; set; }
        public string applicationModuleTitle { get; set; }
        public string menuTitle { get; set; }
        public bool read { get; set; }
        public bool write { get; set; }
        public bool delete { get; set; }
        public bool all { get; set; }
    }
}