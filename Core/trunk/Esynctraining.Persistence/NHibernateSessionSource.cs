namespace Esynctraining.Persistence
{
    using System.Runtime.Remoting.Contexts;
    using global::NHibernate;
    using NHibernate;

    /// <summary>
    /// The n hibernate session source.
    /// </summary>
    [Synchronization]
    public class NHibernateSessionSource : ISessionSource
    {
        #region Fields

        /// <summary>
        /// The session.
        /// </summary>
        private readonly ISession session;

        #endregion

        // 		protected static readonly object NHibernateHashtableKey = new object();
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="NHibernateSessionSource"/> class.
        /// </summary>
        /// <param name="sessionFactory">
        /// The session factory.
        /// </param>
        public NHibernateSessionSource(ISessionFactory sessionFactory)
        {
            this.session = sessionFactory.OpenSession();

            // 			if (Local.Data[NHibernateHashtableKey] == null)
            // 			{
            // 				Local.Data[NHibernateHashtableKey] = sessionFactory.OpenSession();
            // 			}
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
                return this.session; // Local.Data[NHibernateHashtableKey] as ISession;
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
                    this.Session.Dispose();

                    // 					Local.Data[NHibernateHashtableKey] = null;
                }
            }
        }

        #endregion
    }
}