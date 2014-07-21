namespace EdugameCloud.Core.Domain.DTO
{
    using System;
    using System.Runtime.Serialization;

    using EdugameCloud.Core.Domain.Entities;

    using Esynctraining.Core.Extensions;

    /// <summary>
    ///     The company theme DTO.
    /// </summary>
    [DataContract]
    public class CompanyThemeDTO
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="CompanyThemeDTO" /> class.
        /// </summary>
        public CompanyThemeDTO()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompanyThemeDTO"/> class.
        /// </summary>
        /// <param name="companyId">
        /// The company Id.
        /// </param>
        /// <param name="theme">
        /// The theme.
        /// </param>
        public CompanyThemeDTO(int companyId, CompanyTheme theme)
        {
            this.companyThemeId = theme.Return(x => x.Id, Guid.Empty);
            this.companyId = companyId;
            this.logoId = theme.Return(x => x.Logo.Return(i => i.Id, (Guid?)null), null);
            this.headerBackgroundColor = theme.With(x => x.HeaderBackgroundColor);
            this.loginHeaderTextColor = theme.With(x => x.LoginHeaderTextColor);
            this.buttonColor = theme.With(x => x.ButtonColor);
            this.buttonTextColor = theme.With(x => x.ButtonTextColor);
            this.gridHeaderTextColor = theme.With(x => x.GridHeaderTextColor);
            this.gridHeaderBackgroundColor = theme.With(x => x.GridHeaderBackgroundColor);
            this.gridRolloverColor = theme.With(x => x.GridRolloverColor);

            this.popupHeaderBackgroundColor = theme.With(x => x.PopupHeaderBackgroundColor);
            this.popupHeaderTextColor = theme.With(x => x.PopupHeaderTextColor);
            this.questionColor = theme.With(x => x.QuestionColor);
            this.questionHeaderColor = theme.With(x => x.QuestionHeaderColor);
            this.welcomeTextColor = theme.With(x => x.WelcomeTextColor);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the popup header background color.
        /// </summary>
        [DataMember]
        public string popupHeaderBackgroundColor { get; set; }

        /// <summary>
        /// Gets or sets the popup header text color.
        /// </summary>
        [DataMember]
        public string popupHeaderTextColor { get; set; }

        /// <summary>
        /// Gets or sets the question color.
        /// </summary>
        [DataMember]
        public string questionColor { get; set; }

        /// <summary>
        /// Gets or sets the question header color.
        /// </summary>
        [DataMember]
        public string questionHeaderColor { get; set; }

        /// <summary>
        /// Gets or sets the welcome text color.
        /// </summary>
        [DataMember]
        public string welcomeTextColor { get; set; }

        /// <summary>
        /// Gets or sets the button color.
        /// </summary>
        [DataMember]
        public string buttonColor { get; set; }

        /// <summary>
        /// Gets or sets the button text color.
        /// </summary>
        [DataMember]
        public string buttonTextColor { get; set; }

        /// <summary>
        ///     Gets or sets the company theme Id.
        /// </summary>
        [DataMember]
        public Guid companyThemeId { get; set; }

        /// <summary>
        ///     Gets or sets the company.
        /// </summary>
        [DataMember]
        public int companyId { get; set; }

        /// <summary>
        /// Gets or sets the grid header background color.
        /// </summary>
        [DataMember]
        public string gridHeaderBackgroundColor { get; set; }

        /// <summary>
        /// Gets or sets the grid header text color.
        /// </summary>
        [DataMember]
        public string gridHeaderTextColor { get; set; }

        /// <summary>
        /// Gets or sets the grid rollover color.
        /// </summary>
        [DataMember]
        public string gridRolloverColor { get; set; }

        /// <summary>
        /// Gets or sets the header background color.
        /// </summary>
        [DataMember]
        public string headerBackgroundColor { get; set; }

        /// <summary>
        /// Gets or sets the instruction color.
        /// </summary>
        [DataMember]
        public string loginHeaderTextColor { get; set; }

        /// <summary>
        ///     Gets or sets the logo image id.
        /// </summary>
        [DataMember]
        public Guid? logoId { get; set; }

        #endregion
    }
}