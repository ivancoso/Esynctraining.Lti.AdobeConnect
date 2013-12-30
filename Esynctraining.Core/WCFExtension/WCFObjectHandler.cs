namespace Esynctraining.Core.WCFExtension
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.ServiceModel;
    using System.ServiceModel.Configuration;

    using Weborb.Activation;
    using Weborb.Config;
    using Weborb.Exceptions;
    using Weborb.Handler;
    using Weborb.Inspection;
    using Weborb.Util;
    using Weborb.Util.Logging;

    /// <summary>
    /// The WCF object handler.
    /// </summary>
    public class WCFObjectHandler : IInspectionHandler, IInvocationHandler
    {
        #region Constants

        /// <summary>
        /// The AMF binding.
        /// </summary>
        public const string AMFBinding = "amfBinding";

        /// <summary>
        /// The service directory.
        /// </summary>
        public const string ServicesDirectory = "services";

        /// <summary>
        /// The service extension.
        /// </summary>
        public const string ServiceExtension = ".svc";

        /// <summary>
        /// The name.
        /// </summary>
        public const string HandlerName = "WCF Object Handler";

        #endregion

        #region Static Fields

        /// <summary>
        /// The services to contracts.
        /// </summary>
        private static readonly IDictionary<Type, Type> ServicesToContracts = new Dictionary<Type, Type>();

        #endregion

        #region Fields

        /// <summary>
        /// The created factories.
        /// </summary>
        private readonly IList<Type> createdFactories = new List<Type>();

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name
        {
            get
            {
                return HandlerName;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The inspect.
        /// </summary>
        /// <param name="targetObject">
        /// The target object.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceDescriptor"/>.
        /// </returns>
        public ServiceDescriptor inspect(string targetObject)
        {
            try
            {
                Type serviceType = TypeLoader.LoadType(targetObject);
                if (!this.TryGenerateServiceFile(serviceType))
                {
                    return null;
                }

                ServiceDescriptor descriptor = ClassInspector.inspectClass(ServicesToContracts[serviceType]);
                if (Log.isLogging(LoggingConstants.INFO))
                {
                    Log.log(LoggingConstants.INFO, "WCF Object handler has successfully inspected target service");
                }

                return descriptor;
            }
            catch (Exception exception)
            {
                string message = exception.Message;
                if (string.IsNullOrWhiteSpace(message))
                {
                    message = "Unable to inspect WCF object with id " + targetObject;
                }

                if (Log.isLogging(LoggingConstants.ERROR))
                {
                    Log.log(LoggingConstants.ERROR, string.Format("{0} {1}", message, exception));
                }

                return null;
            }
        }

        /// <summary>
        /// The invoke.
        /// </summary>
        /// <param name="serviceName">
        /// The service name.
        /// </param>
        /// <param name="function">
        /// The function.
        /// </param>
        /// <param name="arguments">
        /// The arguments.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The <see cref="Value"/>.
        /// </returns>
        public virtual Value invoke(string serviceName, string function, object[] arguments, RequestContext context)
        {
            object obj2 = null;
            try
            {
                if (Log.isLogging(LoggingConstants.INFO))
                {
                    Log.log(LoggingConstants.INFO, serviceName + " " + function + " to invoke in WCFObjectHandler");
                }

                Type serviceType = TypeLoader.LoadType(serviceName);
                if (!this.TryGenerateServiceFile(serviceType))
                {
                    return null;
                }

                obj2 = this.InstantinateProxy(serviceType, typeof(WCFChannelFactory<,>));
                MethodInfo method = obj2.GetType().GetMethod(function);
                return new Value(Invocation.invoke(obj2, method, arguments));
            }
            catch (Exception exception)
            {
                if (obj2 != null)
                {
                    ((ICommunicationObject)obj2).Abort();
                }

                if (Log.isLogging(LoggingConstants.EXCEPTION))
                {
                    Log.log(
                        LoggingConstants.EXCEPTION, 
                        "WCFObjectHandler unnable to handle invoke request due to ", 
                        exception);
                }

                return
                    new Value(
                        new ServiceException(
                            "Exception during invocation method " + function + ". " + exception.Message, exception));
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The exists pure WCF endpoint.
        /// </summary>
        /// <param name="endpointsElements">
        /// The endpoints elements.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        protected bool ExistsPureWCFEndpoint(ServiceEndpointElementCollection endpointsElements)
        {
            return endpointsElements.Cast<ServiceEndpointElement>().Any(element => element.BehaviorConfiguration != AMFBinding);
        }

        /// <summary>
        /// Get instance of a proxy.
        /// </summary>
        /// <param name="serviceType">
        /// The service type.
        /// </param>
        /// <param name="channelFactory">
        /// The channel factory.
        /// </param>
        /// <returns>
        /// The proxy object.
        /// </returns>
        protected object InstantinateProxy(Type serviceType, Type channelFactory)
        {
            Type type;
            lock (ServicesToContracts)
            {
                type = ServicesToContracts[serviceType];
            }

            this.RegisterProxyFactory(type, serviceType, channelFactory);
            IActivator activator = new SessionActivator();
            object[] customAttributes = serviceType.GetCustomAttributes(typeof(ServiceBehaviorAttribute), false);
            if (customAttributes.Length > 0)
            {
                var attribute = (ServiceBehaviorAttribute)customAttributes[0];
                if (attribute.InstanceContextMode == InstanceContextMode.Single)
                {
                    activator = new ApplicationActivator();
                }
                else if (attribute.InstanceContextMode == InstanceContextMode.PerCall)
                {
                    activator = new RequestActivator();
                }
            }

            return activator.Activate(type);
        }

        /// <summary>
        /// The register proxy factory.
        /// </summary>
        /// <param name="contractType">
        /// The contract type.
        /// </param>
        /// <param name="serviceType">
        /// The service type.
        /// </param>
        /// <param name="channelFactory">
        /// The channel factory.
        /// </param>
        protected virtual void RegisterProxyFactory(Type contractType, Type serviceType, Type channelFactory)
        {
            lock (this.createdFactories)
            {
                if (!this.createdFactories.Contains(serviceType))
                {
                    var factory = (ChannelFactory)Activator.CreateInstance(channelFactory.MakeGenericType(new[] { contractType, serviceType }));
                    ORBConfig.GetInstance().getObjectFactories().AddServiceObjectFactory(contractType.FullName, (IServiceObjectFactory)factory);
                    this.createdFactories.Add(serviceType);
                }
                else
                {
                    var objFactories = ORBConfig.GetInstance().getObjectFactories();
                    var soFactory = (ChannelFactory)objFactories._GetServiceObjectFactory(contractType.FullName);
                    if (soFactory.State == CommunicationState.Faulted 
                        || soFactory.State == CommunicationState.Closed
                        || soFactory.State == CommunicationState.Closing)
                    {
                        objFactories.RemoveServiceFactoryFor(contractType.FullName);
                        var factory =
                        (ChannelFactory)Activator.CreateInstance(channelFactory.MakeGenericType(new[] { contractType, serviceType }));
                        ORBConfig.GetInstance().getObjectFactories().AddServiceObjectFactory(contractType.FullName, (IServiceObjectFactory)factory);
                    }
                }
            }
        }

        /// <summary>
        /// The try generate service file.
        /// </summary>
        /// <param name="serviceType">
        /// The service type.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        protected virtual bool TryGenerateServiceFile(Type serviceType)
        {
            if (serviceType == null)
            {
                return false;
            }

            lock (ServicesToContracts)
            {
                if (ServicesToContracts.ContainsKey(serviceType))
                {
                    return true;
                }

                ServiceElementCollection services = WCFUtil.GetServices();
                if (services != null)
                {
                    foreach (ServiceElement element in services)
                    {
                        if ((element.Name == serviceType.FullName) && this.ExistsPureWCFEndpoint(element.Endpoints))
                        {
                            string path = Paths.GetWebORBPath() + Path.DirectorySeparatorChar + ServicesDirectory;
                            if (!Directory.Exists(path))
                            {
                                Directory.CreateDirectory(path);
                            }

                            Type type = TypeLoader.LoadType(element.Endpoints[0].Contract);
                            string serviceFile = string.Concat(new object[] { path, Path.DirectorySeparatorChar, serviceType.FullName, ServiceExtension });
                            ServicesToContracts[serviceType] = type;
                            if (!File.Exists(serviceFile))
                            {
                                TextWriter writer = new StreamWriter(serviceFile);
                                writer.WriteLine("<%@ ServiceHost Language='C#' Service='{0}'%>", serviceType.FullName);
                                writer.Close();
                                if (Log.isLogging(LoggingConstants.INFO))
                                {
                                    Log.log(LoggingConstants.INFO, string.Format("SVC file was generated for service {0}", serviceType.FullName));
                                }
                            }

                            return true;
                        }
                    }

                    if (Log.isLogging(LoggingConstants.INFO))
                    {
                        Log.log(LoggingConstants.INFO, "Object " + serviceType.FullName + " has no WCF contract implemented");
                    }
                }

                return false;
            }
        }

        #endregion
    }
}