namespace EdugameCloud.Persistence
{
    using System;
    using System.Collections;
    using System.Runtime.Remoting.Contexts;

    using Esynctraining.Core.Utils;

    using NHibernate;

    /// <summary>
    /// The n hibernate session source.
    /// </summary>
    [Synchronization]
    public class NHibernateSessionSource : ISessionSource
    {
        #region Inner Class

        public static class Local
        {
            private static readonly ILocalData Current = new LocalData();


            public static ILocalData Data => Current;


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
                        return threadHashtable ?? (threadHashtable = new Hashtable());
                    }
                }

                #endregion

                public object this[object key]
                {
                    get { return LocalHashtable[key]; }
                    set { LocalHashtable[key] = value; }
                }

                public void Clear()
                {
                    LocalHashtable.Clear();
                }

            }

        }

        #endregion

        protected static readonly object NHibernateHashtableKey = new object();


        public ISession Session
        {
            get
            {
                return Local.Data[NHibernateHashtableKey] as ISession;
            }
        }


        public NHibernateSessionSource(ISessionFactory sessionFactory)
        {
            if (Local.Data[NHibernateHashtableKey] == null)
            {
                Local.Data[NHibernateHashtableKey] = sessionFactory.OpenSession();
            }
        }


        public virtual void Dispose()
        {
            if (this.Session != null)
            {
                try
                {
                    this.Session.Flush();
                }
                finally
                {
                    try
                    {
                        if (this.Session != null)
                        {
                            this.Session.Dispose();
                        }
                    }
                    finally 
                    {
                        Local.Data[NHibernateHashtableKey] = null;    
                    }
                }
            }
        }

    }

}