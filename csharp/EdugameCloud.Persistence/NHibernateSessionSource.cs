using System;
using System.Runtime.Remoting.Contexts;
using NHibernate;

namespace EdugameCloud.Persistence
{
    //todo: merge to one class with NHibernateSessionWebSource
    [Synchronization]
    public class NHibernateSessionSource : ISessionSource
    {
        public ISession Session { get; }

        public NHibernateSessionSource(ISessionFactory sessionFactory)
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
                    Session?.Dispose();
                }
            }
        }
    }
}