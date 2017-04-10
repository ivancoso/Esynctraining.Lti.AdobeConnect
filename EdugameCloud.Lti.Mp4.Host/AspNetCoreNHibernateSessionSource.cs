using System;
using System.Collections;
using System.Runtime.Remoting.Contexts;
using EdugameCloud.Persistence;
using Esynctraining.Core.Utils;
using Microsoft.AspNetCore.Http;
using NHibernate;

namespace EdugameCloud.Lti.Mp4.Host
{
    [Synchronization]
    internal sealed class AspNetCoreNHibernateSessionSource : ISessionSource
    {
        #region Inner Class

        public static class Local
        {
            private static readonly object LocalDataHashtableKey = new object();


            public static ILocalData Data { get; } = IoC.Resolve<ILocalData>();


            public sealed class LocalData : ILocalData
            {
                private readonly IHttpContextAccessor _contextAccessor;


                public object this[object key]
                {
                    get { return LocalHashtable[key]; }
                    set { LocalHashtable[key] = value; }
                }

                private Hashtable LocalHashtable
                {
                    get
                    {
                        var webHashtable = _contextAccessor.HttpContext.Items[LocalDataHashtableKey] as Hashtable;
                        if (webHashtable == null)
                        {
                            _contextAccessor.HttpContext.Items[LocalDataHashtableKey] = webHashtable = new Hashtable();
                        }

                        return webHashtable;
                    }
                }


                public LocalData(IHttpContextAccessor contextAccessor)
                {
                    _contextAccessor = contextAccessor ?? throw new ArgumentNullException(nameof(contextAccessor));
                }


                public void Clear()
                {
                    LocalHashtable.Clear();
                }

            }

        }

        #endregion

        private static readonly object NHibernateHashtableKey = new object();


        public NHibernate.ISession Session => Local.Data[NHibernateHashtableKey] as NHibernate.ISession;


        public AspNetCoreNHibernateSessionSource(ISessionFactory sessionFactory)
        {
            if (sessionFactory == null)
                throw new ArgumentNullException(nameof(sessionFactory));

            if (Local.Data[NHibernateHashtableKey] == null)
            {
                Local.Data[NHibernateHashtableKey] = sessionFactory.OpenSession();
            }
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
                    try
                    {
                        if (Session != null)
                        {
                            Session.Dispose();
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
