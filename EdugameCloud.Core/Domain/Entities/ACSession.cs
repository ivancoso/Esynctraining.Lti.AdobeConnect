namespace EdugameCloud.Core.Domain.Entities
{
    using System;

    using Esynctraining.Core.Domain.Entities;

    using Iesi.Collections.Generic;

    /// <summary>
    /// AC Session
    /// </summary>
    public class ACSession : Entity
    {
        /// <summary>
        /// The group discussions.
        /// </summary>
        private ISet<SNGroupDiscussion> groupDiscussions = new HashedSet<SNGroupDiscussion>();

        /// <summary>
        /// The SN members.
        /// </summary>
        private ISet<SNMember> members = new HashedSet<SNMember>();

        /// <summary>
        /// The survey results.
        /// </summary>
        private ISet<SurveyResult> surveyResults = new HashedSet<SurveyResult>();

        /// <summary>
        /// The quiz results.
        /// </summary>
        private ISet<QuizResult> quizResults = new HashedSet<QuizResult>();

        /// <summary>
        /// The quiz results.
        /// </summary>
        private ISet<TestResult> testResults = new HashedSet<TestResult>();

        /// <summary>
        /// The applet results.
        /// </summary>
        private ISet<AppletResult> appletResults = new HashedSet<AppletResult>();

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
        public virtual ISet<SNGroupDiscussion> GroupDiscussions
        {
            get
            {
                return this.groupDiscussions;
            }

            set
            {
                this.groupDiscussions = value;
            }
        }

        /// <summary>
        /// Gets or sets the SN members.
        /// </summary>
        public virtual ISet<SNMember> SNMembers
        {
            get
            {
                return this.members;
            }

            set
            {
                this.members = value;
            }
        }

        /// <summary>
        /// Gets or sets the quiz results.
        /// </summary>
        public virtual ISet<QuizResult> QuizResults
        {
            get
            {
                return this.quizResults;
            }

            set
            {
                this.quizResults = value;
            }
        }

        /// <summary>
        /// Gets or sets the test results.
        /// </summary>
        public virtual ISet<TestResult> TestResults
        {
            get
            {
                return this.testResults;
            }

            set
            {
                this.testResults = value;
            }
        }

        /// <summary>
        /// Gets or sets the survey results.
        /// </summary>
        public virtual ISet<SurveyResult> SurveyResults
        {
            get
            {
                return this.surveyResults;
            }

            set
            {
                this.surveyResults = value;
            }
        }

        /// <summary>
        /// Gets or sets the applet results.
        /// </summary>
        public virtual ISet<AppletResult> AppletResults
        {
            get
            {
                return this.appletResults;
            }
            set
            {
                this.appletResults = value;
            }
        }

        #endregion
    }
}