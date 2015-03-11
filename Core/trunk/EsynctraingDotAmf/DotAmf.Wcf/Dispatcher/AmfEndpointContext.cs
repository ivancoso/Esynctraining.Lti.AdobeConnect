// --------------------------------------------------------------------------------------------------------------------
// <copyright company="" file="AmfEndpointContext.cs">
//   
// </copyright>
// <summary>
//   AMF endpoint context.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace DotAmf.ServiceModel.Dispatcher
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.ServiceModel.Description;

    using DotAmf.Data;
    using DotAmf.Serialization;
    using DotAmf.ServiceModel.Messaging;

    /// <summary>
    ///     AMF endpoint context.
    /// </summary>
    internal sealed class AmfEndpointContext : IDisposable
    {
        #region Fields

        /// <summary>
        ///     Registered contracts.
        /// </summary>
        private readonly List<Type> _contracts;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AmfEndpointContext"/> class.
        /// </summary>
        /// <param name="endpoint">
        /// The endpoint.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        public AmfEndpointContext(ServiceEndpoint endpoint)
        {
            if (endpoint == null)
            {
                throw new ArgumentNullException("endpoint");
            }

            this._contracts = new List<Type>();
            this.ResolveContracts(endpoint);

            this.ServiceEndpoint = endpoint;
            this.AmfSerializer = new DataContractAmfSerializer(typeof(AmfPacket), this._contracts);
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     AMF serializer.
        /// </summary>
        public DataContractAmfSerializer AmfSerializer { get; private set; }

        /// <summary>
        ///     Related service endpoint.
        /// </summary>
        public ServiceEndpoint ServiceEndpoint { get; private set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose() { }

        #endregion

        #region Methods

        /// <summary>
        /// Check if type is a valid data contract.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private static bool IsValidDataContract(Type type)
        {
            return DataContractHelper.IsDataContract(type);
        }

        /// <summary>
        /// Register data contract.
        /// </summary>
        /// <param name="type">
        /// Type to register.
        /// </param>
        /// <exception cref="InvalidDataContractException">
        /// Invalid data contract.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Type already registered.
        /// </exception>
        private void AddContract(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (this.IsContractRegistered(type))
            {
                throw new InvalidOperationException("Type already registered.");
            }

            if (!IsValidDataContract(type))
            {
                throw new InvalidDataContractException(
                    string.Format("Type '{0}' is not a valid data contract.", type.FullName));
            }

            this._contracts.Add(type);

            if (type.IsClass)
            {
                IEnumerable<Type> memberTypes = ProcessClass(type);

                foreach (Type subtype in memberTypes)
                {
                    if (this.IsContractRegistered(subtype) || !IsValidDataContract(subtype))
                    {
                        continue;
                    }

                    this.AddContract(subtype);
                }
            }
        }

        /// <summary>
        /// Check if type is registered as a data contract.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool IsContractRegistered(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            return this._contracts.Contains(type);
        }

        /// <summary>
        /// The process class.
        /// </summary>
        /// <param name="classType">
        /// The class type.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        /// <exception cref="ArgumentException">
        /// </exception>
        private static IEnumerable<Type> ProcessClass(Type classType)
        {
            if (classType == null)
            {
                throw new ArgumentNullException("classType");
            }

            if (!classType.IsClass)
            {
                throw new ArgumentException(string.Format("Type '{0}' is not class.", classType), "classType");
            }

            IEnumerable<Type> properties = from property in DataContractHelper.GetContractProperties(classType)
                                           select property.Value.PropertyType;
            var types = properties.ToList();

            // Handle complex types
            for (int i = 0; i < types.Count; i++)
            {
                Type type = types[i];

                // Type is an array
                if (type.IsArray && type.HasElementType)
                {
                    types[i] = type.GetElementType();
                }
            }

            return types;
        }

        /// <summary>
        /// Resolve endpoint contracts.
        /// </summary>
        /// <param name="endpoint">
        /// The endpoint.
        /// </param>
        private void ResolveContracts(ServiceEndpoint endpoint)
        {
            // Add default contracts
            this.AddContract(typeof(AbstractMessage));
            this.AddContract(typeof(AcknowledgeMessage));
            this.AddContract(typeof(CommandMessage));
            this.AddContract(typeof(ErrorMessage));
            this.AddContract(typeof(RemotingMessage));

            // Add endpoint contract's types
            var types = new List<Type>();

            // Get return types and methods parameters
            foreach (MethodInfo method in endpoint.Contract.Operations.Select(operation => operation.SyncMethod))
            {
                types.Add(method.ReturnType);
                types.AddRange(method.GetParameters().Select(param => param.ParameterType));
            }

            // Get operation fault contracts
            IEnumerable<Type> faultTypes = from operation in endpoint.Contract.Operations
                                           from fault in operation.Faults
                                           select fault.DetailType;

            types.AddRange(faultTypes);

            // Handle complex types
            for (int i = 0; i < types.Count; i++)
            {
                Type type = types[i];

                // Type is an array
                if (type.IsArray && type.HasElementType)
                {
                    types[i] = type.GetElementType();
                }
            }

            // Remove duplicates and invalid types
            IEnumerable<Type> validtypes =
                types.Distinct().Where(IsValidDataContract).Where(type => !this.IsContractRegistered(type));

            // Register all valid types
            foreach (Type type in validtypes)
            {
                this.AddContract(type);
            }
        }

        #endregion

    }

}