namespace EdugameCloud.Core.Domain.Entities
{
    using System.Collections.Generic;
    using Esynctraining.Core.Domain.Entities;
    using Iesi.Collections.Generic;

    /// <summary>
    ///     The applet item.
    /// </summary>
    public class AppletItem : Entity
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the applet name.
        /// </summary>
        public virtual string AppletName { get; set; }

        /// <summary>
        /// Gets or sets the document xml.
        /// </summary>
        public virtual string DocumentXML { get; set; }

        /// <summary>
        /// Gets or sets the results.
        /// </summary>
        public virtual IList<AppletResult> Results { get; protected set; }

        /// <summary>
        /// Gets or sets the sub module item.
        /// </summary>
        public virtual SubModuleItem SubModuleItem { get; set; }

        #endregion

        public AppletItem()
        {
            Results = new List<AppletResult>();
        }

    }

}