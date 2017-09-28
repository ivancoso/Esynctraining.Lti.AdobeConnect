namespace EdugameCloud.Persistence
{
    using System;
    using System.Collections;
    using System.Runtime.Remoting.Contexts;
    using System.Web;
    using Esynctraining.Core.Utils;
    using NHibernate;

    /// <summary>
    /// The hibernate session web source.
    /// </summary>
    [Synchronization]
    public sealed class NHibernateSessionWebSource : ISessionSource
    {
        #region Inner Class

        public static class Local
        {
            private static readonly object LocalDataHashtableKey = new object();

            private static readonly ILocalData Current = new LocalData();


            public static ILocalData Data => Current;
            
            private class LocalData : ILocalData
            {
                private static Hashtable LocalHashtable
                {
                    get
                    {
                        var currentContext = HttpContext.Current;
                        if (currentContext == null)
                            throw new InvalidOperationException("HttpContext.Current == null");

                        var webHashtable = currentContext.Items[LocalDataHashtableKey] as Hashtable;
                        if (webHashtable == null)
                        {
                            currentContext.Items[LocalDataHashtableKey] = webHashtable = new Hashtable();
                        }

                        return webHashtable;
                    }
                }

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

        private static readonly object NHibernateHashtableKey = new object();


        public ISession Session => Local.Data[NHibernateHashtableKey] as ISession;


        public NHibernateSessionWebSource(ISessionFactory sessionFactory)
        {
            if (Local.Data[NHibernateHashtableKey] == null)
            {
                Local.Data[NHibernateHashtableKey] = sessionFactory.OpenSession();
            }
        }


        public void Dispose()
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