namespace UMISModuleApi.dto.response
{
    public class UserForList
    {
        public int userID { get; set; }
        public int roleId { get; set; }
        public string roleTitle { get; set; }
        public string moduleTitle { get; set; }
        public string empName { get; set; }
        public string loginName { get; set; }
        public string active { get; set; }
    }
}