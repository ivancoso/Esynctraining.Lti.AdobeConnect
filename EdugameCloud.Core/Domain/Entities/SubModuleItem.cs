namespace EdugameCloud.Core.Domain.Entities
{
    using System;
    using System.Collections.Generic;
    using Esynctraining.Core.Domain.Entities;

    public class SubModuleItem : Entity
    {
        #region Public Properties

        public virtual IList<ACSession> ACSessions { get; protected set; }

        public virtual IList<SubModuleItemTheme> Themes { get; protected set; }

        public virtual IList<AppletItem> AppletItems { get; protected set; }

        public virtual User CreatedBy { get; set; }

        public virtual DateTime DateCreated { get; set; }

        public virtual DateTime DateModified { get; set; }

        public virtual bool? IsActive { get; set; }

        public virtual bool? IsShared { get; set; }

        public virtual User ModifiedBy { get; set; }

        public virtual IList<Question> Questions { get; protected set; }

        public virtual IList<Quiz> Quizes { get; protected set; }

        public virtual IList<Survey> Surveys { get; protected set; }

        public virtual SubModuleCategory SubModuleCategory { get; set; }

        public virtual IList<Test> Tests { get; protected set; }

        public virtual IList<SNProfile> SNProfiles { get; protected set; }

        #endregion

        public SubModuleItem()
        {
            ACSessions = new List<ACSession>();
            Themes = new List<SubModuleItemTheme>();
            AppletItems = new List<AppletItem>();
            SNProfiles = new List<SNProfile>();
            Questions = new List<Question>();
            Quizes = new List<Quiz>();
            Surveys = new List<Survey>();
            Tests = new List<Test>();
        }

    }

}