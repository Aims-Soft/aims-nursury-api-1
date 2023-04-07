using System.ComponentModel.DataAnnotations;

namespace UMISModuleAPI.Models
{
    public class AuthenticateRequest
    {
        // [Required]
        // public string email { get; set; }

        // [Required]
        // public string password { get; set; }

        [Required]
        public string Loginname { get; set; }

        [Required]
        public string hashpassword { get; set; }
        public string SpType { get; set; }
    }
}