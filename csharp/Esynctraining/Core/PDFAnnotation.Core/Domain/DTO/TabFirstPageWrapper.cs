namespace PDFAnnotation.Core.Domain.DTO
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    /// <summary>
    /// The tab first page wrapper.
    /// </summary>
    /// <typeparam name="T">
    /// DTO items
    /// </typeparam>
    [DataContract]
    public class TabFirstPageWrapper<T>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TabFirstPageWrapper{T}"/> class.
        /// </summary>
        public TabFirstPageWrapper()
        {
            this.items = new List<T>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TabFirstPageWrapper{T}"/> class.
        /// </summary>
        /// <param name="items">
        /// The items.
        /// </param>
        /// <param name="totalCount">
        /// The total count.
        /// </param>
        public TabFirstPageWrapper(IEnumerable<T> items, int totalCount)
        {
            this.items = items;
            this.totalCount = totalCount;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the items.
        /// </summary>
        [DataMember]
        public IEnumerable<T> items { get; set; }

        /// <summary>
        ///     Gets or sets the total.
        /// </summary>
        [DataMember]
        public int totalCount { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The convert.
        /// </summary>
        /// <param name="converter">
        /// The converter.
        /// </param>
        /// <typeparam name="V">
        /// DTO type
        /// </typeparam>
        /// <returns>
        /// The <see cref="TabFirstPageWrapper{V}"/>.
        /// </returns>
        public TabFirstPageWrapper<V> Convert<V>(Func<T, V> converter)
        {
            return new TabFirstPageWrapper<V>
                       {
                           items = this.items.Select(converter).ToList(), 
                           totalCount = this.totalCount
                       };
        }

        #endregion
    }
}