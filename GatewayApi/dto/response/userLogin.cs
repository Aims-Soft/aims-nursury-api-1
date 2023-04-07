using System.Text.Json.Serialization;


namespace gatewayapi.Entities
{
    public class UserLogin
    {
        public int id { get; set; }
        public string fullName { get; set; }
        public string loginName { get; set; }
        public string userType { get; set; }
        public string region { get; set; }
    }

}