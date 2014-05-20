namespace EdugameCloud.Core.RTMP
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Castle.Core.Logging;

    using EdugameCloud.Core.Constants;

    using Esynctraining.Core.Extensions;
    using Esynctraining.Core.Utils;

    using Weborb.Messaging.Api;
    using Weborb.Messaging.Api.Service;
    using Weborb.Messaging.Server.Adapter;
    using Weborb.Types;

    /// <summary>
    ///     The DB changes notify.
    /// </summary>
    public class DBChangesNotifier : ApplicationAdapter
    {
        #region Static Fields

        /// <summary>
        /// The locker.
        /// </summary>
        private static readonly object locker = new object();

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The app connect.
        /// </summary>
        /// <param name="conn">
        /// The conn.
        /// </param>
        /// <param name="parms">
        /// The parameters.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public override bool appConnect(IConnection conn, object[] parms)
        {
            var logger = IoC.Resolve<ILogger>();
            try
            {
                int companyId;
                if (parms.Length > 0 && parms[0] is IAdaptingType
                    && int.TryParse(((IAdaptingType)parms[0]).adapt(typeof(int)).With(x => x.ToString()), out companyId))
                {
                    conn.setAttribute(ClientConstants.CompanyIdAttribute, companyId);
                    logger.Info(string.Format("RTMP Client connected: {0}", companyId));
                }
                else
                {
                    logger.Error(string.Format("Connected client doesn't have companyId. Params count: {0} First param: {1}", parms.Length, parms.FirstOrDefault()));
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Connected client doesn't have companyId. Params count: {0}; First param: {1}; Error: {2}", parms.Length, parms.FirstOrDefault(), ex));
            }

            return base.appConnect(conn, parms);
        }

        /// <summary>
        /// The notify domain object deleted.
        /// </summary>
        /// <param name="doType">
        /// The do type.
        /// </param>
        /// <param name="companyId">
        /// The company id.
        /// </param>
        /// <param name="doId">
        /// The do id.
        /// </param>
        public void notifyDomainObjectDeleted(string doType, int companyId, int doId)
        {
            this.pushData("notifyDomainObjectDeleted", companyId, doType, doId);
            var logger = IoC.Resolve<ILogger>();
            logger.Info(string.Format("RTMP event got fired: {0}, companyId={1}, id={2}", doType, companyId, doId));
        }

        /// <summary>
        /// The notify domain object inserted.
        /// </summary>
        /// <param name="doType">
        /// The do type.
        /// </param>
        /// <param name="companyId">
        /// The company id.
        /// </param>
        /// <param name="doId">
        /// The do id.
        /// </param>
        public void notifyDomainObjectInserted(string doType, int companyId, int doId)
        {
            this.pushData("notifyDomainObjectInserted", companyId, doType, doId);
            var logger = IoC.Resolve<ILogger>();
            logger.Info(string.Format("RTMP event got fired: {0}, companyId={1}, id={2}", doType, companyId, doId));
        }

        #endregion

        #region Methods

        /// <summary>
        /// The push data.
        /// </summary>
        /// <param name="methodName">
        /// The method name.
        /// </param>
        /// <param name="companyId">
        /// The company Id.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        private void pushData(string methodName, int companyId, params object[] args)
        {
            lock (locker)
            {
                if (this.scope.getClients().Count == 0)
                {
                    return;
                }

                IEnumerator<IConnection> connections = this.scope.getConnections();
                while (connections.MoveNext())
                {
                    IConnection connection = connections.Current;
                    int clientCompanyId;
                    object companyIdAttr = connection.getAttribute(ClientConstants.CompanyIdAttribute);
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

        #endregion
    }

    // ReSharper restore InconsistentNaming
}