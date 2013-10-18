namespace Esynctraining.Core.Utils
{
    using System;
    using System.Collections;
    using System.Web;

    /// <summary>
    /// The local.
    /// </summary>
    public static class Local
    {
        #region Static Fields

        /// <summary>
        /// The local data hash table key.
        /// </summary>
        private static readonly object LocalDataHashtableKey = new object();

        /// <summary>
        /// The current instance of local data.
        /// </summary>
        private static readonly ILocalData Current = new LocalData();

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the data.
        /// </summary>
        public static ILocalData Data
        {
            get
            {
                return Current;
            }
        }

        /// <summary>
        /// Gets a value indicating whether running in web.
        /// </summary>
        public static bool RunningInWeb
        {
            get
            {
                return HttpContext.Current != null;
            }
        }

        #endregion

        /// <summary>
        /// The local data.
        /// </summary>
        private class LocalData : ILocalData
        {
            #region Static Fields

            /// <summary>
            /// The thread hash table.
            /// </summary>
            [ThreadStatic]
            private static Hashtable threadHashtable;

            #endregion

            #region Properties

            /// <summary>
            /// Gets the local hash table.
            /// </summary>
            private static Hashtable LocalHashtable
            {
                get
                {
                    if (!RunningInWeb)
                    {
                        return threadHashtable ?? (threadHashtable = new Hashtable());
                    }

                    var webHashtable = HttpContext.Current.Items[LocalDataHashtableKey] as Hashtable;
                    if (webHashtable == null)
                    {
                        HttpContext.Current.Items[LocalDataHashtableKey] = webHashtable = new Hashtable();
                    }

                    return webHashtable;
                }
            }

            #endregion

            #region Public Indexers

            /// <summary>
            /// The this.
            /// </summary>
            /// <param name="key">
            /// The key.
            /// </param>
            /// <returns>
            /// The <see cref="object"/>.
            /// </returns>
            public object this[object key]
            {
                get
                {
                    return LocalHashtable[key];
                }

                set
                {
                    LocalHashtable[key] = value;
                }
            }

            #endregion

            #region Public Methods and Operators

            /// <summary>
            /// The clear.
            /// </summary>
            public void Clear()
            {
                LocalHashtable.Clear();
            }

            #endregion
        }
    }
}