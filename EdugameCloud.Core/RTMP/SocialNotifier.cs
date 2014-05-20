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
    ///     The social changes notify.
    /// </summary>
    public class SocialNotifier : ApplicationAdapter
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
                string key;
                if (parms.Length > 0 && parms[0] is IAdaptingType
                    && !string.IsNullOrWhiteSpace(key = ((IAdaptingType)parms[0]).adapt(typeof(string)).With(x => x.ToString())))
                {
                    conn.setAttribute(ClientConstants.SocialKeyAttribute, key);
                    logger.Info(string.Format("RTMP Social Client connected: {0}", key));
                }
                else
                {
                    logger.Error(string.Format("Social. Connected client doesn't have key. Params count: {0} First param: {1}", parms.Length, parms.FirstOrDefault()));
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Social. Connected client doesn't have key. Params count: {0}; First param: {1}; Error: {2}", parms.Length, parms.FirstOrDefault(), ex));
            }

            return base.appConnect(conn, parms);
        }

        /// <summary>
        /// The notify domain object deleted.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="vo">
        /// The vo.
        /// </param>
        public void notifyTokenSecretObtained(string key, string vo)
        {
            const string MethodName = "notifyTokenSecretObtained";
            this.pushData(key, MethodName, vo);
            var logger = IoC.Resolve<ILogger>();
            logger.Info(string.Format("RTMP social event got fired: {0}, key={1}, vo={2}", MethodName, key, vo));
        }

        #endregion

        #region Methods

        /// <summary>
        /// The push data.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="methodName">
        /// The method name.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        private void pushData(string key, string methodName, params object[] args)
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
                    object socialKey = connection.getAttribute(ClientConstants.SocialKeyAttribute);
                    if (socialKey != null && socialKey.ToString().Equals(key))
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