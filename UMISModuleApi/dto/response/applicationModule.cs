using System.Text.Json.Serialization;


namespace UMISModuleAPI.Entities
{
    public class ApplicationModule
    {
        public int applicationModuleID { get; set; }
        public string applicationModuleTitle { get; set; }
        public string applicationModuledescription { get; set; }
    }
}