using NHibernate;
using NHibernate.Cfg;
using eSyncTraining.Database.Entities;

namespace eSyncTraining.Database
{
    public static class NHibernateHelper
    {
        private static ISessionFactory _sessionFactory;

        private static ISessionFactory SessionFactory
        {
            get
            {
                if (_sessionFactory == null)
                {
                    _sessionFactory = CreateSessionFactory();
                }

                return _sessionFactory;
            }
        }

        private static ISessionFactory CreateSessionFactory()
        {
            var config = new Configuration();

            config.Configure();

            //config.AddClass(typeof (UserLoginHistoryEntity));

            var sessionFactory = config.BuildSessionFactory();

            //var schema = new SchemaExport(config);
            //schema.Create(true, true);

            return sessionFactory;
        }

        public static ISession OpenSession()
        {
            return SessionFactory.OpenSession();
        }
    }
}



