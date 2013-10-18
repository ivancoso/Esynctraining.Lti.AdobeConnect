namespace Esynctraining.Core.Weborb.WCFExtension
{
    using System;
    using System.Configuration;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Configuration;
    using System.ServiceModel.Description;
    using System.Web;
    using System.Web.Configuration;
    using Esynctraining.Core.Extensions;

    using global::Weborb.Util;
    using global::Weborb.Util.Logging;

    /// <summary>
    /// The WCF channel factory.
    /// </summary>
    /// <typeparam name="TChannel">
    /// Type of channel
    /// </typeparam>
    /// <typeparam name="TService">
    /// Type of service
    /// </typeparam>
    public class WCFChannelFactory<TChannel, TService> : AbstractChannelFactory<TChannel>
        where TChannel : class
    {
        #region Methods

        /// <summary>
        /// The create description.
        /// </summary>
        /// <returns>
        /// The <see cref="System.ServiceModel.Description.ServiceEndpoint"/>.
        /// </returns>
        protected override ServiceEndpoint CreateDescription()
        {
            ServiceEndpoint description = base.CreateDescription();
            string domainAppVirtualPath = HttpRuntime.AppDomainAppVirtualPath;
            Configuration config = WebConfigurationManager.OpenWebConfiguration(domainAppVirtualPath);
            if (Log.isLogging(LoggingConstants.INFO))
            {
                Log.log(LoggingConstants.INFO, "Configuration file: " + config.FilePath);
            }

            ServiceModelSectionGroup sectionGroup = ServiceModelSectionGroup.GetSectionGroup(config);

            if (sectionGroup != null)
            {
                ServiceEndpointElement endpoint = null;
                if ((endpoint = this.FindEndpoint(sectionGroup)) != null)
                {
                    description.Binding = this.CreateBinding(endpoint.Binding, endpoint.BindingConfiguration, sectionGroup);
                    string uri = config.AppSettings.Settings["BaseWCFServiceUrl"] != null ?
                        string.Format("{0}{1}.svc/{2}", config.AppSettings.Settings["BaseWCFServiceUrl"].With(x => x.Value), typeof(TService).FullName, endpoint.Address) :
                        string.Format("{0}{1}/services/{2}.svc/{3}", ThreadContext.currentRequest().Return(x => x.Url.GetLeftPart(UriPartial.Authority), "http://localhost"), domainAppVirtualPath, typeof(TService).FullName, endpoint.Address);

                    if (Log.isLogging(LoggingConstants.DEBUG))
                    {
                        Log.log(LoggingConstants.DEBUG, "WCF Service address : " + uri);
                    }

                    description.Address = new EndpointAddress(uri);
                    description.Name = endpoint.Contract;
                    return description;
                }
                if (Log.isLogging(LoggingConstants.ERROR))
                {
                    Log.log(LoggingConstants.ERROR, string.Format("WCF Service Endpoint for service {0} wasn't found", typeof(TService).FullName));
                }
            }

            return null;
        }

        /// <summary>
        /// The find endpoint.
        /// </summary>
        /// <param name="serviceModel">
        /// The service model.
        /// </param>
        /// <returns>
        /// The <see cref="System.ServiceModel.Configuration.ServiceEndpointElement"/>.
        /// </returns>
        private ServiceEndpointElement FindEndpoint(ServiceModelSectionGroup serviceModel)
        {
            ServiceEndpointElement endpoint = null;
            bool serviceFound = false;
            foreach (ServiceElement service in serviceModel.Services.Services)
            {
                if (service.Name == typeof(TService).FullName)
                {
                    foreach (ServiceEndpointElement enp in service.Endpoints)
                    {
                        if (enp.Contract == typeof(TChannel).FullName)
                        {
                            endpoint = enp;
                            if (Log.isLogging(LoggingConstants.DEBUG))
                            {
                                Log.log(LoggingConstants.DEBUG, "Service " + endpoint.Contract + " was founded with binding " + endpoint.Binding);
                            }
                            serviceFound = true;
                        }

                        if (serviceFound)
                        {
                            break;
                        }
                    }

                    if (serviceFound)
                    {
                        break;
                    }
                }
            }

            return endpoint;
        }

        /// <summary>
        /// The create binding.
        /// </summary>
        /// <param name="bindingName">
        /// The binding name.
        /// </param>
        /// <param name="bindingConfiguration">
        /// The binding configuration.
        /// </param>
        /// <param name="group">
        /// The group.
        /// </param>
        /// <returns>
        /// The <see cref="System.ServiceModel.Channels.Binding"/>.
        /// </returns>
        private Binding CreateBinding(string bindingName, string bindingConfiguration, ServiceModelSectionGroup group)
        {
            foreach (IBindingConfigurationElement configurationElement in group.Bindings[bindingName].ConfiguredBindings)
            {
                if (configurationElement.Name == bindingConfiguration)
                {
                    Binding binding = this.GetBinding(configurationElement);
                    if (!(configurationElement is CustomBindingElement))
                    {
                        if (Log.isLogging(LoggingConstants.DEBUG))
                        {
                            Log.log(LoggingConstants.DEBUG, "WCF Binding configuration was found and applied");
                        }

                        configurationElement.ApplyConfiguration(binding);
                    }

                    return binding;
                }
            }

            if (Log.isLogging(LoggingConstants.DEBUG))
            {
                Log.log(LoggingConstants.DEBUG, "WCF Binding with no configuration will be used");
            }

            return this.GetBinding(bindingName);
        }

        /// <summary>
        /// The get binding.
        /// </summary>
        /// <param name="configurationElement">
        /// The configuration element.
        /// </param>
        /// <returns>
        /// The <see cref="System.ServiceModel.Channels.Binding"/>.
        /// </returns>
        private Binding GetBinding(IBindingConfigurationElement configurationElement)
        {
            if (configurationElement is CustomBindingElement)
            {
                return new CustomBinding(
                    (BindingElement)new BinaryMessageEncodingBindingElement(),
                    SecurityBindingElement.CreateUserNameOverTransportBindingElement(),
                    new NonSslAuthHttpTransportBindingElement());
            }

            if (configurationElement is BasicHttpBindingElement)
            {
                return new BasicHttpBinding();
            }

            if (configurationElement is NetMsmqBindingElement)
            {
                return new NetMsmqBinding();
            }

            if (configurationElement is NetNamedPipeBindingElement)
            {
                return new NetNamedPipeBinding();
            }

            if (configurationElement is NetPeerTcpBindingElement)
            {
                return new NetPeerTcpBinding();
            }

            if (configurationElement is NetTcpBindingElement)
            {
                return new NetTcpBinding();
            }

            if (configurationElement is WSDualHttpBindingElement)
            {
                return new WSDualHttpBinding();
            }

            if (configurationElement is WSHttpBindingElement)
            {
                return new WSHttpBinding();
            }

            if (configurationElement is WSFederationHttpBindingElement)
            {
                return new WSFederationHttpBinding();
            }

            if (configurationElement is WS2007FederationHttpBinding)
            {
                return new WS2007FederationHttpBinding();
            }

            if (configurationElement is WS2007HttpBinding)
            {
                return new WS2007HttpBinding();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// The get binding.
        /// </summary>
        /// <param name="bindingName">
        /// The binding name.
        /// </param>
        /// <returns>
        /// The <see cref="System.ServiceModel.Channels.Binding"/>.
        /// </returns>
        private Binding GetBinding(string bindingName)
        {
            switch (bindingName)
            {
                case "basicHttpBinding":
                    return new BasicHttpBinding();
                case "netMsmqBinding":
                    return new NetMsmqBinding();
                case "netNamedPipeBinding":
                    return new NetNamedPipeBinding();
                case "wsFederationHttpBinding":
                    return new WSFederationHttpBinding();
                case "wsDualHttpBinding":
                    return new WSDualHttpBinding();
                case "netPeerTcpBinding":
                    return new NetPeerTcpBinding();
                case "netTcpBinding":
                    return new NetTcpBinding();
                case "wsHttpBinding":
                    return new WSHttpBinding();
                case "ws2007FederatedHttpBinding":
                    return new WS2007FederationHttpBinding();
                case "ws2007HttpBinding":
                    return new WS2007HttpBinding();
                case "customBinding":
                    return new CustomBinding();
                default:
                    return null;
            }
        }

        #endregion
    }
}