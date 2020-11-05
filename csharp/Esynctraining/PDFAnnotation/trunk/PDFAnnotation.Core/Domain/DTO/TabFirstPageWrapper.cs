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

        public TabFirstPageWrapper()
        {
            this.items = new List<T>();
        }

        public TabFirstPageWrapper(IEnumerable<T> items, int totalCount)
        {
            this.items = items;
            this.totalCount = totalCount;
        }

        #endregion

        #region Public Properties

        [DataMember]
        public IEnumerable<T> items { get; set; }

        [DataMember]
        public int totalCount { get; set; }

        #endregion

        #region Public Methods and Operators

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