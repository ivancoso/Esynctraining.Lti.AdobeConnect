namespace EdugameCloud.RTMPHandler
{
    using System;
    using System.Threading;

    using EdugameCloud.Core.Constants;

    using Esynctraining.Core.Extensions;
    using Esynctraining.Core.Utils;

    using Weborb.Messaging.Api;
    using Weborb.Messaging.Api.Service;
    using Weborb.Messaging.Server.Adapter;

    using System.Linq;

    /// <summary>
    /// The db changes notifier.
    /// </summary>
    // ReSharper disable InconsistentNaming
    public class DBChangesNotifier : ApplicationAdapter
    {
        private static object _locker = new object();

        /// <summary>
        /// The app connect.
        /// </summary>
        /// <param name="conn">
        /// The conn.
        /// </param>
        /// <param name="parms">
        /// The parms.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public override bool appConnect(IConnection conn, object[] parms)
        {
            int companyId;
            var logger = IoC.Resolve<Castle.Core.Logging.ILogger>();
            try
            {
                if (parms.Length > 0
                && parms[0] is Weborb.Types.IAdaptingType
                && int.TryParse(((Weborb.Types.IAdaptingType)parms[0]).adapt(typeof(int)).With(x => x.ToString()), out companyId))
                {
                    conn.setAttribute(ClientConstants.CompanyIdAttribute, companyId);
                    logger.Info("RTMP Client connected: " + companyId);
                }
                else
                {
                    
                    logger.Error("Connected client doesn't have companyId. Params count: " + parms.Length + " First param: " + parms.FirstOrDefault());
                }
            }
            catch (Exception ex)
            {
                logger.Error("Connected client doesn't have companyId. Params count: " + parms.Length + " First param: " + parms.FirstOrDefault());
            }
            

            return base.appConnect(conn, parms);;
        }

        public void notifyDomainObjectInserted(string doType, int companyId, int doId)
        {
            this.pushData("notifyDomainObjectInserted", companyId, doType, doId);
        }

        public void notifyDomainObjectDeleted(string doType, int companyId, int doId)
        {
            this.pushData("notifyDomainObjectDeleted", companyId, doType, doId);
        }

        /// <summary>
        /// The push data.
        /// </summary>
        /// <param name="methodName">
        /// The method name.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        private void pushData(string methodName, int companyId, params object[] args)
        {
            lock (_locker)
            {
                if (scope.getClients().Count == 0)
                {
                    return;
                }

                var connections = scope.getConnections();
                while (connections.MoveNext())
                {
                    var connection = connections.Current;
                    int clientCompanyId;
                    var companyIdAttr = connection.getAttribute(ClientConstants.CompanyIdAttribute);
                    if (companyIdAttr != null && int.TryParse(companyIdAttr.ToString(), out clientCompanyId)
                        && clientCompanyId == companyId)
                    {
                        var capableConnection = connection as IServiceCapableConnection;
                        if (capableConnection != null)
                        {
                            capableConnection.invoke(methodName, args);
                        }
                    }
                }
            }
        }
    }
    // ReSharper restore InconsistentNaming
}
