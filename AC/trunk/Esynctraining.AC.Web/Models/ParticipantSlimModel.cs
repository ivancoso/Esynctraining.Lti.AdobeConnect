namespace eSyncTraining.Web.Models
{
    using System.ComponentModel.DataAnnotations;

    using Esynctraining.AC.Provider.Entities;

    public class ParticipantSlimModel
    {
        [Display(Name = "Id")]
        public string PrincipalId { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Login")]
        public string Login { get; set; }

        [Display(Name = "Permission")]
        public PermissionId PermissionId { get; set; }
    }
}
