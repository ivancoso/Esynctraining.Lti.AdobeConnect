// --------------------------------------------------------------------------------------------------------------------
// <copyright company="" file="AmfBindingExtensionElement.cs">
//   
// </copyright>
// <summary>
//   AMF binding extension.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace DotAmf.ServiceModel.Configuration
{
    using System;
    using System.Configuration;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Configuration;

    using DotAmf.Data;
    using DotAmf.ServiceModel.Channels;

    /// <summary>
    ///     AMF binding extension.
    /// </summary>
    /// <remarks>
    ///     Enables the use of a custom <c>BindingElement</c> implementation from a machine or application configuration
    ///     file.
    /// </remarks>
    public sealed class AmfBindingExtensionElement : BindingElementExtensionElement
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="AmfBindingExtensionElement" /> class.
        ///     Constructor.
        /// </summary>
        public AmfBindingExtensionElement()
        {
            this.Version = AmfVersion.Amf3;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the <c>System.Type</c> object that represents the custom binding element.
        /// </summary>
        public override Type BindingElementType
        {
            get
            {
                return typeof(AmfEncodingBindingElement);
            }
        }

        /// <summary>
        ///     AMF version.
        /// </summary>
        [ConfigurationProperty("version", DefaultValue = AmfVersion.Amf3)]
        public AmfVersion Version { get; set; }

        #endregion

        #region Methods

        /// <summary>
        ///     Returns a custom binding element object.
        /// </summary>
        /// <returns>
        ///     The <see cref="BindingElement" />.
        /// </returns>
        protected override BindingElement CreateBindingElement()
        {
            var options = new AmfEncodingOptions { AmfVersion = this.Version, UseContextSwitch = true };

            return new AmfEncodingBindingElement(options);
        }

        #endregion
    }
}