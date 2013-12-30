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
        #region Static Fields

        /// <summary>
        /// The n hibernate hashtable key.
        /// </summary>
        protected static readonly object NHibernateHashtableKey = new object();

        #endregion

        #region Fields

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="NHibernateSessionSource"/> class.
        /// </summary>
        /// <param name="sessionFactory">
        /// The session factory.
        /// </param>
        public NHibernateSessionSource(ISessionFactory sessionFactory)
        {
            if (Local.Data[NHibernateHashtableKey] == null)
            {
                Local.Data[NHibernateHashtableKey] = sessionFactory.OpenSession();
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the session.
        /// </summary>
        public ISession Session
        {
            get
            {
                return Local.Data[NHibernateHashtableKey] as ISession;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The dispose.
        /// </summary>
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

        #endregion
    }
}