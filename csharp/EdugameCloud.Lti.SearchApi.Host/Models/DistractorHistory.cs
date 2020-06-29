using System;
using System.Collections.Generic;

namespace EdugameCloud.Lti.SearchApi.Host.Models
{
    public partial class DistractorHistory
    {
        public int DistractoryHistoryId { get; set; }
        public int DistractorId { get; set; }
        public int? QuestionId { get; set; }
        public int? ImageId { get; set; }
        public string Distractor { get; set; }
        public int DistractorOrder { get; set; }
        public string Score { get; set; }
        public bool? IsCorrect { get; set; }
        public int? CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public bool? IsActive { get; set; }

        public virtual Distractor DistractorNavigation { get; set; }
    }
}
