using System;
using System.Runtime.Remoting.Contexts;
using EdugameCloud.Persistence;
using NHibernate;

namespace EdugameCloud.Lti.Api.Host
{
    [Synchronization]
    internal sealed class AspNetCoreNHibernateSessionSource : ISessionSource
    {
        public ISession Session { get; private set; }


        public AspNetCoreNHibernateSessionSource(ISessionFactory sessionFactory)
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
