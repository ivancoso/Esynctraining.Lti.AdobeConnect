namespace EdugameCloud.Core.Domain.Entities
{
    using Esynctraining.Core.Domain.Entities;

    using Iesi.Collections.Generic;

    /// <summary>
    ///     The applet item.
    /// </summary>
    public class AppletItem : Entity
    {
        #region Fields

        /// <summary>
        /// The results.
        /// </summary>
        private ISet<AppletResult> results = new HashedSet<AppletResult>();

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the applet name.
        /// </summary>
        public virtual string AppletName { get; set; }

        /// <summary>
        ///     Gets or sets the document xml.
        /// </summary>
        public virtual string DocumentXML { get; set; }

        /// <summary>
        /// Gets or sets the results.
        /// </summary>
        public virtual ISet<AppletResult> Results
        {
            get
            {
                return this.results;
            }

            set
            {
                this.results = value;
            }
        }

        /// <summary>
        ///     Gets or sets the sub module item.
        /// </summary>
        public virtual SubModuleItem SubModuleItem { get; set; }

        #endregion
    }
}