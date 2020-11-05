namespace eSyncTraining.Web.Models
{
    using System.ComponentModel.DataAnnotations;

    public class UserInfoModel
    {
        [Display(Name = "Login")]
        public string Login { get; set; }

        [Display(Name = "Name")]
        public string Username { get; set; }
    }
}
