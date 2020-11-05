namespace Esynctraining.Core.Weborb.WCFExtension
{
    using System;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Configuration;

    /// <summary>
    /// The non SSL authentication transport element.
    /// </summary>
    public class NonSslAuthTransportElement : HttpTransportElement
    {
        /// <summary>
        /// Gets the binding element type.
        /// </summary>
        public override Type BindingElementType
        {
            get
            {
                return typeof(NonSslAuthHttpTransportBindingElement);
            }
        }

        /// <summary>
        /// The create default binding element.
        /// </summary>
        /// <returns>
        /// The <see cref="System.ServiceModel.Channels.TransportBindingElement"/>.
        /// </returns>
        protected override TransportBindingElement CreateDefaultBindingElement()
        {
            return new NonSslAuthHttpTransportBindingElement();
        }
    }
}
