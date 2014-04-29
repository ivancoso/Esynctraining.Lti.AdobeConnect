namespace EdugameCloud.Core.Domain.Entities
{
    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    ///     The sub module item theme.
    /// </summary>
    public class CompanyTheme : EntityGuid
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the button color.
        /// </summary>
        public virtual string ButtonColor { get; set; }

        /// <summary>
        /// Gets or sets the button text color.
        /// </summary>
        public virtual string ButtonTextColor { get; set; }

        /// <summary>
        /// Gets or sets the grid header background color.
        /// </summary>
        public virtual string GridHeaderBackgroundColor { get; set; }

        /// <summary>
        /// Gets or sets the grid header text color.
        /// </summary>
        public virtual string GridHeaderTextColor { get; set; }

        /// <summary>
        /// Gets or sets the grid rollover color.
        /// </summary>
        public virtual string GridRolloverColor { get; set; }

        /// <summary>
        /// Gets or sets the header background color.
        /// </summary>
        public virtual string HeaderBackgroundColor { get; set; }

        /// <summary>
        /// Gets or sets the logo.
        /// </summary>
        public virtual File Logo { get; set; }

        #endregion
    }
}