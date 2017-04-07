namespace EdugameCloud.Persistence
{
    using System;
    using System.Runtime.Remoting.Contexts;

    using Esynctraining.Core.Utils;

    using NHibernate;

    /// <summary>
    /// The n hibernate session source.
    /// </summary>
    [Synchronization]
    public class NHibernateSessionSource : ISessionSource
    {
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