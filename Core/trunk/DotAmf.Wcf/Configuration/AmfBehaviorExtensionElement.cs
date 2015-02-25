// --------------------------------------------------------------------------------------------------------------------
// <copyright company="" file="AmfBehaviorExtensionElement.cs">
//   
// </copyright>
// <summary>
//   AMF endpoint behavior extension.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace DotAmf.ServiceModel.Configuration
{
    using System;
    using System.ServiceModel.Configuration;

    using DotAmf.ServiceModel.Description;

    /// <summary>
    ///     AMF endpoint behavior extension.
    /// </summary>
    /// <remarks>
    ///     Represents a configuration element that contains sub-elements that specify behavior extensions,
    ///     which enable the user to customize service or endpoint behaviors.
    /// </remarks>
    public sealed class AmfBehaviorExtensionElement : BehaviorExtensionElement
    {
        #region Public Properties

        /// <summary>
        ///     Gets the type of behavior.
        /// </summary>
        public override Type BehaviorType
        {
            get
            {
                return typeof(AmfEndpointBehavior);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Creates a behavior extension based on the current configuration settings.
        /// </summary>
        /// <returns>
        ///     The <see cref="object" />.
        /// </returns>
        protected override object CreateBehavior()
        {
            return new AmfEndpointBehavior();
        }

        #endregion
    }
}