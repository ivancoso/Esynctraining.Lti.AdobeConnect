

namespace PDFAnnotation.Core.Domain.DTO
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    /// <summary>
    /// The page wrapper.
    /// </summary>
    /// <typeparam name="T">
    /// DTO items
    /// </typeparam>
    [DataContract]
    public class TopicPageWrapper
    {
  
        #region Public Properties

        /// <summary>
        ///     Gets or sets the items.
        /// </summary>
        [DataMember]
        public TopicDTO[] items { get; set; }


        /// <summary>
        ///     Gets or sets the total.
        /// </summary>
        [DataMember]
        public int totalCount { get; set; }

        #endregion

    }
}
