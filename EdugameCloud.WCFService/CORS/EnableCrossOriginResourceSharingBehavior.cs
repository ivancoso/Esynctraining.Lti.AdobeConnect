namespace EdugameCloud.WCFService.CORS
{
    using System;
    using System.Collections.Generic;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Configuration;
    using System.ServiceModel.Description;
    using System.ServiceModel.Dispatcher;

    /// <summary>
    ///     The enable cross origin resource sharing behavior.
    /// </summary>
    public class EnableCrossOriginResourceSharingBehavior : BehaviorExtensionElement, IEndpointBehavior
    {
        #region Public Properties

        /// <summary>
        /// Gets the behavior type.
        /// </summary>
        public override Type BehaviorType
        {
            get
            {
                return typeof(EnableCrossOriginResourceSharingBehavior);
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The add binding parameters.
        /// </summary>
        /// <param name="endpoint">
        /// The endpoint.
        /// </param>
        /// <param name="bindingParameters">
        /// The binding parameters.
        /// </param>
        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {

        }

        /// <summary>
        /// The apply client behavior.
        /// </summary>
        /// <param name="endpoint">
        /// The endpoint.
        /// </param>
        /// <param name="clientRuntime">
        /// The client runtime.
        /// </param>
        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
        }

        /// <summary>
        /// The apply dispatch behavior.
        /// </summary>
        /// <param name="endpoint">
        /// The endpoint.
        /// </param>
        /// <param name="endpointDispatcher">
        /// The endpoint dispatcher.
        /// </param>
        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            var requiredHeaders = new Dictionary<string, string>
                                      {
                                          { "Access-Control-Allow-Origin", "*" }, 
                                          {
                                              "Access-Control-Request-Method", 
                                              "POST, GET, PUT, DELETE, OPTIONS"
                                          }, 
                                          {
                                              "Access-Control-Allow-Headers", 
                                              "X-Requested-With, Content-Type, SOAPAction, Accept, Origin"
                                          }, 
                                          { "Access-Control-Max-Age", "1728000" }
                                      };

            endpointDispatcher.DispatchRuntime.MessageInspectors.Add(new CustomHeaderMessageInspector(requiredHeaders));
        }

        /// <summary>
        /// The validate.
        /// </summary>
        /// <param name="endpoint">
        /// The endpoint.
        /// </param>
        public void Validate(ServiceEndpoint endpoint)
        {
        }

        #endregion

        #region Methods

        /// <summary>
        ///     The create behavior.
        /// </summary>
        /// <returns>
        ///     The <see cref="object" />.
        /// </returns>
        protected override object CreateBehavior()
        {
            return new EnableCrossOriginResourceSharingBehavior();
        }

        #endregion
    }
}