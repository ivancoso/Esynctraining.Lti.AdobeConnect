using System;
using System.Collections.Generic;

namespace EdugameClaud.Lti.SearchApi.Host.Models
{
    public partial class Acsession
    {
        public Acsession()
        {
            AppletResult = new HashSet<AppletResult>();
            QuizResult = new HashSet<QuizResult>();
            SngroupDiscussion = new HashSet<SngroupDiscussion>();
            Snmember = new HashSet<Snmember>();
            SurveyResult = new HashSet<SurveyResult>();
            TestResult = new HashSet<TestResult>();
        }

        public int AcSessionId { get; set; }
        public int SubModuleItemId { get; set; }
        public int UserId { get; set; }
        public int AcUserModeId { get; set; }
        public int AccountId { get; set; }
        public string MeetingUrl { get; set; }
        public int ScoId { get; set; }
        public DateTime DateCreated { get; set; }
        public int LanguageId { get; set; }
        public int Status { get; set; }
        public bool? IncludeAcEmails { get; set; }

        public virtual AcuserMode AcUserMode { get; set; }
        public virtual Language Language { get; set; }
        public virtual SubModuleItem SubModuleItem { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<AppletResult> AppletResult { get; set; }
        public virtual ICollection<QuizResult> QuizResult { get; set; }
        public virtual ICollection<SngroupDiscussion> SngroupDiscussion { get; set; }
        public virtual ICollection<Snmember> Snmember { get; set; }
        public virtual ICollection<SurveyResult> SurveyResult { get; set; }
        public virtual ICollection<TestResult> TestResult { get; set; }
    }
}
