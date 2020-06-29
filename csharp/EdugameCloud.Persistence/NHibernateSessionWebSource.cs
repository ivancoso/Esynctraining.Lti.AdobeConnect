namespace EdugameCloud.Persistence
{
    using System;
    using System.Runtime.Remoting.Contexts;
    using NHibernate;

    /// <summary>
    /// The hibernate session web source.
    /// </summary>
    [Synchronization]
    public sealed class NHibernateSessionWebSource : ISessionSource
    {
        public ISession Session { get; private set; }


        public NHibernateSessionWebSource(ISessionFactory sessionFactory)
        {
            if (sessionFactory == null)
                throw new ArgumentNullException(nameof(sessionFactory));

            Session = sessionFactory.OpenSession();
        }


        public void Dispose()
        {
            if (Session != null)
            {
                try
                {
                    Session.Flush();
                }
                finally
                {
                    if (Session != null)
                    {
                        Session.Dispose();
                    }
                }
            }
        }

    }

}