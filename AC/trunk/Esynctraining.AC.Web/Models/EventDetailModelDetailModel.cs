namespace eSyncTraining.Web.Models
{
    using System.ComponentModel.DataAnnotations;

    public class EventDetailModel : ScoDetailModelBase
    {
        [Display(Name = "Related Meeting")]
        public string MeetingId { get; set; }
    }
}
