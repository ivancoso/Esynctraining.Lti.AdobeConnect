namespace EdugameCloud.WCFService.ExtendedHttpBehavior
{
    using System;
    using System.ServiceModel.Configuration;
    using System.ServiceModel.Description;
    using System.ServiceModel.Dispatcher;

    /// <summary>
    ///     The extended web http behavior.
    /// </summary>
    public sealed class ExtendedWebHttpBehavior : WebHttpBehavior
    {
        #region Methods

        /// <summary>
        /// The get query string converter.
        /// </summary>
        /// <param name="operationDescription">
        /// The operation description.
        /// </param>
        /// <returns>
        /// The <see cref="QueryStringConverter"/>.
        /// </returns>
        protected override QueryStringConverter GetQueryStringConverter(OperationDescription operationDescription)
        {
            return new ExtendedQueryStringConverter();
        }

        #endregion
    }

    /// <summary>
    /// The http behavior extension element.
    /// </summary>
    public sealed class HttpBehaviorExtensionElement : BehaviorExtensionElement
    {
        #region Public Properties

        /// <summary>
        /// Gets the behavior type.
        /// </summary>
        public override Type BehaviorType
        {
            get
            {
                return typeof(ExtendedWebHttpBehavior);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The create behavior.
        /// </summary>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        protected override object CreateBehavior()
        {
            return new ExtendedWebHttpBehavior();
        }

        #endregion
    }
}