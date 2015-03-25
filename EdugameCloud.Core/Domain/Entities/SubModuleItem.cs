namespace EdugameCloud.Core.Domain.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Esynctraining.Core.Domain.Entities;
    using Iesi.Collections.Generic;

    /// <summary>
    ///     The sub module item
    /// </summary>
    public class SubModuleItem : Entity
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the ac sessions.
        /// </summary>
        public virtual IList<ACSession> ACSessions { get; protected set; }

        /// <summary>
        ///     Gets or sets the ac sessions.
        /// </summary>
        public virtual IList<SubModuleItemTheme> Themes { get; protected set; }

        /// <summary>
        /// Gets or sets the applet items.
        /// </summary>
        public virtual IList<AppletItem> AppletItems { get; protected set; }

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
        public virtual IList<Question> Questions { get; protected set; }

        /// <summary>
        /// Gets or sets the quiz set.
        /// </summary>
        public virtual IList<Quiz> Quizes { get; protected set; }

        /// <summary>
        /// Gets or sets the quiz set.
        /// </summary>
        public virtual IList<Survey> Surveys { get; protected set; }

        /// <summary>
        ///     Gets or sets the sub module category.
        /// </summary>
        public virtual SubModuleCategory SubModuleCategory { get; set; }

        /// <summary>
        /// Gets or sets the tests.
        /// </summary>
        public virtual IList<Test> Tests { get; protected set; }

        /// <summary>
        /// Gets or sets the tests.
        /// </summary>
        public virtual IList<SNProfile> SNProfiles { get; protected set; }

        #endregion
    }
}