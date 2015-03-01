namespace EdugameCloud.Core.Domain.DTO
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The paged applet items DTO.
    /// </summary>
    [DataContract]
    public class PagedAppletItemsDTO
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the objects.
        /// </summary>
        [DataMember]
        public AppletItemDTO[] objects { get; set; }

        /// <summary>
        /// Gets or sets the total count.
        /// </summary>
        [DataMember]
        public int totalCount { get; set; }

        #endregion
    }
}