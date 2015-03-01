namespace EdugameCloud.Core.Domain.DTO
{
    using System.Runtime.Serialization;

    using EdugameCloud.Core.Domain.Entities;

    using Esynctraining.Core.Extensions;

    /// <summary>
    ///     The applet item DTO.
    /// </summary>
    [DataContract]
    public class AppletItemDTO
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AppletItemDTO"/> class.
        /// </summary>
        public AppletItemDTO()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppletItemDTO"/> class.
        /// </summary>
        /// <param name="result">
        /// The result.
        /// </param>
        public AppletItemDTO(AppletItem result)
        {
            this.appletItemId = result.Id;
            this.subModuleItemId = result.SubModuleItem.Return(x => x.Id, (int?)null);
            this.appletName = result.AppletName;
            this.documentXML = result.DocumentXML;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the applet name.
        /// </summary>
        [DataMember]
        public string appletName { get; set; }

        /// <summary>
        ///     Gets or sets the document xml.
        /// </summary>
        [DataMember]
        public string documentXML { get; set; }

        /// <summary>
        ///     Gets or sets the sub module item.
        /// </summary>
        [DataMember]
        public int? subModuleItemId { get; set; }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        [DataMember]
        public int appletItemId { get; set; }

        #endregion
    }
}