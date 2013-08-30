namespace eSyncTraining.Web.Models
{
    using System.ComponentModel.DataAnnotations;

    public class MeetingDetailModel : ScoDetailModelBase
    {
        [Display(Name = "Permission")]
        public string PermissionId { get; set; }
    }
}
