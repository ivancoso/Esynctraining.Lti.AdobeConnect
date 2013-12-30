namespace Esynctraining.Core.WCFExtension
{
    using System;
    using System.Collections.Generic;
    using System.ServiceModel;
    using System.ServiceModel.Channels;

    using Weborb.Security;
    using Weborb.Util;
    using Weborb.Util.Logging;

    /// <summary>
    /// The abstract channel factory.
    /// </summary>
    /// <typeparam name="TChannel">
    /// Channel type
    /// </typeparam>
    public class AbstractChannelFactory<TChannel> : ChannelFactory<TChannel>, IServiceObjectFactory
        where TChannel : class
    {
        #region Static Fields

        /// <summary>
        ///     The created channels.
        /// </summary>
        private static IDictionary<Type, List<TChannel>> createdChannels = new Dictionary<Type, List<TChannel>>();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes static members of the <see cref="AbstractChannelFactory{TChannel}"/> class. 
        /// </summary>
        static AbstractChannelFactory()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="AbstractChannelFactory{TChannel}" /> class.
        /// </summary>
        public AbstractChannelFactory()
            : base(typeof(TChannel))
        {
            this.InitializeEndpoint((string)null, null);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractChannelFactory{TChannel}"/> class.
        /// </summary>
        /// <param name="binding">
        /// The binding.
        /// </param>
        /// <param name="endpoint">
        /// The endpoint.
        /// </param>
        public AbstractChannelFactory(Binding binding, EndpointAddress endpoint)
            : base(typeof(TChannel))
        {
            this.InitializeEndpoint(binding, endpoint);
        }

        /// <summary>
        ///     Finalizes an instance of the <see cref="AbstractChannelFactory{TChannel}" /> class.
        /// </summary>
        ~AbstractChannelFactory()
        {
            try
            {
                List<TChannel> list = createdChannels[typeof(TChannel)];
                foreach (TChannel channel in list)
                {
                    if (((ICommunicationObject)channel).State != CommunicationState.Closed)
                    {
                        try
                        {
                            if (Log.isLogging(LoggingConstants.DEBUG))
                            {
                                Log.log(LoggingConstants.DEBUG, string.Format("Channel to service of type {0} is closing or aborting", typeof(TChannel).FullName));
                            }

                            ((ICommunicationObject)channel).Close();
                        }
                        catch (Exception)
                        {
                            ((ICommunicationObject)channel).Abort();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (!Log.isLogging(LoggingConstants.EXCEPTION))
                {
                    return;
                }

                Log.log(LoggingConstants.EXCEPTION, ex);
            }
            finally
            {
                // ISSUE: explicit finalizer call
                // base.Finalize();
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The create object.
        /// </summary>
        /// <returns>
        ///     The <see cref="object" />.
        /// </returns>
        public object createObject()
        {
            Credentials credentials = ThreadContext.currentCallerCredentials();
            if (credentials != null)
            {
                try
                {
                    if (this.State == CommunicationState.Created || this.State == CommunicationState.Opening)
                    {
                        this.Credentials.UserName.UserName = credentials.userid;
                        this.Credentials.UserName.Password = credentials.password;
                    }
//                    else if (this.Credentials != null && this.Credentials.UserName.UserName != credentials.userid)
//                    {
//                        throw new ApplicationException("WebOrb Sucks: factory user name: " + this.Credentials.UserName.UserName + " channel user name: " + credentials.userid);
//                    }
                }
                catch (InvalidOperationException ex)
                {
                    if (ex.Message == "Object is read-only.")
                    {
                        var clientCredentials = new System.ServiceModel.Description.ClientCredentials();
                        clientCredentials.UserName.UserName = credentials.userid;
                        clientCredentials.UserName.Password = credentials.password;
                        bool found = false;
                        for (int i = 0; i < this.Endpoint.Behaviors.Count; i++)
                        {
                            if (this.Endpoint.Behaviors[i] is System.ServiceModel.Description.ClientCredentials)
                            {
                                this.Endpoint.Behaviors.RemoveAt(i);
                                this.Endpoint.Behaviors.Add(clientCredentials);
                                found = true;
                                break;
                            }
                        }
                        if (!found)
                        {
                            //todo think about it
//                            this.Endpoint.Behaviors.RemoveAt(1);
//                            this.Endpoint.Behaviors.Add(clientCredentials);
                        }

                    }
                }
                
            }

            TChannel channel = this.CreateChannel();
            lock (createdChannels)
            {
                InitializeCreatedChannelsCollection();
                ClearFaultedChannelsAndInsertCurrent(channel);
            }

            return channel;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The clear faulted channels and insert current.
        /// </summary>
        /// <param name="channel">
        /// The channel.
        /// </param>
        private static void ClearFaultedChannelsAndInsertCurrent(TChannel channel)
        {
            List<TChannel> channelsByType = createdChannels[typeof(TChannel)];
            int i;
            for (i = 0; i < channelsByType.Count; ++i)
            {
                if (((ICommunicationObject)channelsByType[i]).State == CommunicationState.Faulted)
                {
                    ((ICommunicationObject)channelsByType[i]).Abort();
                    break;
                }
            }

            if (i < channelsByType.Count)
            {
                channelsByType[i] = channel;
            }
            else
            {
                channelsByType.Add(channel);
            }
        }

        /// <summary>
        /// The initialize created channels collection.
        /// </summary>
        private static void InitializeCreatedChannelsCollection()
        {
            if (!createdChannels.ContainsKey(typeof(TChannel)))
            {
                createdChannels[typeof(TChannel)] = new List<TChannel>();
            }
        }

        #endregion
    }
}