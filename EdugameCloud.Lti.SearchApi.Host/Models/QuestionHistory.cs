using System;
using System.Collections.Generic;

namespace EdugameClaud.Lti.SearchApi.Host.Models
{
    public partial class QuestionHistory
    {
        public int QuestionHistoryId { get; set; }
        public int QuestionId { get; set; }
        public int QuestionTypeId { get; set; }
        public int? SubModuleItemId { get; set; }
        public int? ImageId { get; set; }
        public string Question { get; set; }
        public int QuestionOrder { get; set; }
        public string Instruction { get; set; }
        public string IncorrectMessage { get; set; }
        public string Hint { get; set; }
        public int? CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public bool? IsActive { get; set; }

        public virtual Question QuestionNavigation { get; set; }
    }
}
