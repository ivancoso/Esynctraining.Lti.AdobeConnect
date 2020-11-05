namespace eSyncTraining.Web.Models
{
    using System.ComponentModel.DataAnnotations;

    public class PrincipalSlimModel
    {
        [Display(Name = "Id")]
        public string PrincipalId { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Login")]
        public string Login { get; set; }

        [Display(Name = "Has Children")]
        public bool HasChildren { get; set; }
    }
}
