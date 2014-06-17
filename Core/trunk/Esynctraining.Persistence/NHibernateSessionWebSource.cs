namespace Esynctraining.Persistence
{
    using System.Runtime.Remoting.Contexts;
    using System.Web;

    using NHibernate;

    ///<summary>
    ///The n hibernate session web source.
    ///</summary>
    [Synchronization]
    public class NHibernateSessionWebSource : NHibernateSessionSource
    {
        #region Static Fields

        /// <summary>
        /// The lock obj.
        /// </summary>
        private static readonly object lockObj = new object();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="NHibernateSessionWebSource"/> class.
        /// </summary>
        /// <param name="sessionFactory">
        /// The session factory.
        /// </param>
        public NHibernateSessionWebSource(ISessionFactory sessionFactory)
            : base(sessionFactory)
        {
            this.Session.BeginTransaction();
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The dispose.
        /// </summary>
        public override void Dispose()
        {
            if (this.Session != null)
            {
                try
                {
                    ITransaction transaction = this.Session.Transaction;
                    if (transaction.IsActive)
                    {
                        using (transaction)
                        {
                            if (HttpContext.Current.Server.GetLastError() == null)
                            {
                                transaction.Commit();

                            }
                            else
                            {
                                transaction.Rollback();
                            }
                        }
                    }
                }
                finally
                {
                    lock (lockObj)
                    {
                        if (this.Session != null)
                        {
                            this.Session.Dispose();

                            // Local.Data[NHibernateHashtableKey] = null;
                        }
                    }
                }
            }
        }

        #endregion
    }
}