namespace PDFAnnotation.Core.RTMP
{
    using global::Weborb.Messaging.Api;
    using global::Weborb.Messaging.Api.Service;
    using global::Weborb.Messaging.Server.Adapter;

    /// <summary>
    /// The db changes notifier.
    /// </summary>
    // ReSharper disable InconsistentNaming
    public class DBChangesNotifier : ApplicationAdapter
    {
        /// <summary>
        /// The _locker.
        /// </summary>
        private static readonly object _locker = new object();
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
//            int companyId;
//            var logger = IoC.Resolve<Castle.Core.Logging.ILogger>();
//            try
//            {
//                if (parms.Length > 0
//                && parms[0] is Weborb.Types.IAdaptingType
//                && int.TryParse(((Weborb.Types.IAdaptingType)parms[0]).adapt(typeof(int)).With(x => x.ToString()), out companyId))
//                {
//                    conn.setAttribute(ClientConstants.FirmIdAttribute, companyId);
//                    logger.Info("RTMP Client connected: " + companyId);
//                }
//                else
//                {
//                    
//                    logger.Error("Connected client doesn't have companyId. Params count: " + parms.Length + " First param: " + parms.FirstOrDefault());
//                }
//            }
//            catch (Exception ex)
//            {
//                logger.Error("Connected client doesn't have companyId. Params count: " + parms.Length + " First param: " + parms.FirstOrDefault());
//            }
            
            return base.appConnect(conn, parms);;
        }

        /// <summary>
        /// The notify domain object inserted.
        /// </summary>
        /// <param name="doType">
        /// The do type.
        /// </param>
        /// <param name="vo">
        /// The vo.
        /// </param>
        public void notifyDomainObjectSaved(string doType, string vo)
        {
            this.pushData("notifyDomainObjectSaved", doType, vo);
        }

        /// <summary>
        /// The notify domain object deleted.
        /// </summary>
        /// <param name="doType">
        /// The do type.
        /// </param>
        /// <param name="vo">
        /// The vo.
        /// </param>
        public void notifyDomainObjectDeleted(string doType, string vo)
        {
            this.pushData("notifyDomainObjectDeleted", doType, vo);
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
        private void pushData(string methodName, params object[] args)
        {
            lock (_locker)
            {
                if (this.scope.getClients().Count == 0)
                {
                    return;
                }

                var connections = this.scope.getConnections();
                while (connections.MoveNext())
                {
                    var connection = connections.Current;
//                    int clientFirmId;
//                    var companyIdAttr = connection.getAttribute(ClientConstants.FirmIdAttribute);
//                    if (companyIdAttr != null && int.TryParse(companyIdAttr.ToString(), out clientFirmId))
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
