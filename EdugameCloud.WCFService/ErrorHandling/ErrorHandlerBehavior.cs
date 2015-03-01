namespace EdugameCloud.WCFService.ErrorHandling
{
    using System;
    using System.ServiceModel.Configuration;

    /// <summary>
    ///     The error handler behavior.
    /// </summary>
    public class ErrorHandlerBehavior : BehaviorExtensionElement
    {
        #region Public Properties

        /// <summary>
        ///     Gets the behavior type.
        /// </summary>
        public override Type BehaviorType
        {
            get
            {
                return typeof(ErrorServiceBehavior);
            }
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
            return new ErrorServiceBehavior();
        }

        #endregion
    }
}