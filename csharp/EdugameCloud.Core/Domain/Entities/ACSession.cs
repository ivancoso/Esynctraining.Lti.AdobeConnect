namespace EdugameCloud.Core.Domain.Entities
{
    using System;
    using System.Collections.Generic;
    using Esynctraining.Core.Domain.Entities;
    using Iesi.Collections.Generic;

    /// <summary>
    /// AC Session
    /// </summary>
    public class ACSession : Entity
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the AC user mode.
        /// </summary>
        public virtual ACUserMode ACUserMode { get; set; }

        /// <summary>
        /// Gets or sets the account id.
        /// </summary>
        public virtual int AccountId { get; set; }

        /// <summary>
        /// Gets or sets the date created.
        /// </summary>
        public virtual DateTime DateCreated { get; set; }

        /// <summary>
        /// Gets or sets the language.
        /// </summary>
        public virtual Language Language { get; set; }

        /// <summary>
        /// Gets or sets the meeting url.
        /// </summary>
        public virtual string MeetingUrl { get; set; }

        /// <summary>
        /// Gets or sets the sco id.
        /// </summary>
        public virtual int ScoId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether include ac emails.
        /// </summary>
        public virtual bool? IncludeAcEmails { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        public virtual ACSessionStatusEnum Status { get; set; }

        /// <summary>
        /// Gets or sets the sub module item.
        /// </summary>
        public virtual SubModuleItem SubModuleItem { get; set; }

        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        public virtual User User { get; set; }

        /// <summary>
        /// Gets or sets the group discussions.
        /// </summary>
        public virtual IList<SNGroupDiscussion> GroupDiscussions { get; protected set; }

        /// <summary>
        /// Gets or sets the SN members.
        /// </summary>
        public virtual IList<SNMember> SNMembers { get; protected set; }

        /// <summary>
        /// Gets or sets the quiz results.
        /// </summary>
        public virtual IList<QuizResult> QuizResults { get; protected set; }

        /// <summary>
        /// Gets or sets the test results.
        /// </summary>
        public virtual IList<TestResult> TestResults { get; protected set; }

        /// <summary>
        /// Gets or sets the survey results.
        /// </summary>
        public virtual IList<SurveyResult> SurveyResults { get; protected set; }

        /// <summary>
        /// Gets or sets the applet results.
        /// </summary>
        public virtual IList<AppletResult> AppletResults { get; protected set; }

        #endregion

        public ACSession()
        {
            GroupDiscussions = new List<SNGroupDiscussion>();
            SNMembers = new List<SNMember>();
            SurveyResults = new List<SurveyResult>();
            QuizResults = new List<QuizResult>();
            TestResults = new List<TestResult>();
            AppletResults = new List<AppletResult>();
        }

    }

}