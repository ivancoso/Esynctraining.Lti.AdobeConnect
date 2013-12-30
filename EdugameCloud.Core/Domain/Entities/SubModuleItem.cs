namespace EdugameCloud.Core.Domain.Entities
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using Esynctraining.Core.Domain.Entities;

    using Iesi.Collections.Generic;

    /// <summary>
    ///     The sub module item
    /// </summary>
    public class SubModuleItem : Entity
    {
        #region Fields

        /// <summary>
        ///     The AC sessions.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Reviewed. Suppression is OK here.")]
        private ISet<ACSession> acSessions = new HashedSet<ACSession>();

        /// <summary>
        /// The applet items.
        /// </summary>
        private ISet<AppletItem> appletItems = new HashedSet<AppletItem>();

        /// <summary>
        /// The SN profiles.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Reviewed. Suppression is OK here.")]
        private ISet<SNProfile> snProfiles = new HashedSet<SNProfile>(); 

        /// <summary>
        /// The questions.
        /// </summary>
        private ISet<Question> questions = new HashedSet<Question>();

        /// <summary>
        /// The quiz set.
        /// </summary>
        private ISet<Quiz> quizes = new HashedSet<Quiz>();

        /// <summary>
        /// The survey set.
        /// </summary>
        private ISet<Survey> surveys = new HashedSet<Survey>();

        /// <summary>
        /// The tests.
        /// </summary>
        private ISet<Test> tests = new HashedSet<Test>();

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the ac sessions.
        /// </summary>
        public virtual ISet<ACSession> ACSessions
        {
            get
            {
                return this.acSessions;
            }

            set
            {
                this.acSessions = value;
            }
        }

        /// <summary>
        /// Gets or sets the applet items.
        /// </summary>
        public virtual ISet<AppletItem> AppletItems
        {
            get
            {
                return this.appletItems;
            }

            set
            {
                this.appletItems = value;
            }
        }

        /// <summary>
        ///     Gets or sets the created by.
        /// </summary>
        public virtual User CreatedBy { get; set; }

        /// <summary>
        ///     Gets or sets the date created.
        /// </summary>
        public virtual DateTime DateCreated { get; set; }

        /// <summary>
        ///     Gets or sets the date modified.
        /// </summary>
        public virtual DateTime DateModified { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether is active.
        /// </summary>
        public virtual bool? IsActive { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether is shared.
        /// </summary>
        public virtual bool? IsShared { get; set; }

        /// <summary>
        ///     Gets or sets the modified by.
        /// </summary>
        public virtual User ModifiedBy { get; set; }

        /// <summary>
        /// Gets or sets the questions.
        /// </summary>
        public virtual ISet<Question> Questions
        {
            get
            {
                return this.questions;
            }

            set
            {
                this.questions = value;
            }
        }

        /// <summary>
        /// Gets or sets the quiz set.
        /// </summary>
        public virtual ISet<Quiz> Quizes
        {
            get
            {
                return this.quizes;
            }

            set
            {
                this.quizes = value;
            }
        }

        /// <summary>
        /// Gets or sets the quiz set.
        /// </summary>
        public virtual ISet<Survey> Surveys
        {
            get
            {
                return this.surveys;
            }

            set
            {
                this.surveys = value;
            }
        }

        /// <summary>
        ///     Gets or sets the sub module category.
        /// </summary>
        public virtual SubModuleCategory SubModuleCategory { get; set; }

        /// <summary>
        /// Gets or sets the tests.
        /// </summary>
        public virtual ISet<Test> Tests
        {
            get
            {
                return this.tests;
            }

            set
            {
                this.tests = value;
            }
        }

        /// <summary>
        /// Gets or sets the tests.
        /// </summary>
        public virtual ISet<SNProfile> SNProfiles
        {
            get
            {
                return this.snProfiles;
            }

            set
            {
                this.snProfiles = value;
            }
        }

        #endregion
    }
}