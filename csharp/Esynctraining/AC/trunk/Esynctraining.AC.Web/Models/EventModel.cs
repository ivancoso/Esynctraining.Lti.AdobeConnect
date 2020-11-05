namespace eSyncTraining.Web.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class EventModel
    {
        [Display(Name = "Sco Id")]
        public string ScoId { get; set; }

        [Display(Name = "Tree Id")]
        public int TreeId { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Domain name")]
        public string DomainName { get; set; }

        [Display(Name = "Begins at")]
        public DateTime BeginDate { get; set; }

        [Display(Name = "Ends at")]
        public DateTime EndDate { get; set; }

        [Display(Name = "Path")]
        public string Path { get; set; }

        [Display(Name = "Expired")]
        public bool IsExpired { get; set; }

        [Display(Name = "Duration")]
        public TimeSpan Duration { get; set; }
    }
}
