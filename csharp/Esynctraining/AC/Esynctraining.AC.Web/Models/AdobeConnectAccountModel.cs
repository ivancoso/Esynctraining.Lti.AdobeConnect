namespace eSyncTraining.Web.Models
{
    using System.ComponentModel.DataAnnotations;

    public class AdobeConnectLoginModel
    {
        [Required]
        [Display(Name = "Login")]
        public string Login { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Hello, ")]
        public string Username { get; set; }
    }
}
