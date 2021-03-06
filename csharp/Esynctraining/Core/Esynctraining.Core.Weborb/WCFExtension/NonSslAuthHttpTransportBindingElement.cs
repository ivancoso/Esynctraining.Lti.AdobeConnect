namespace Esynctraining.Core.Weborb.WCFExtension
{
    using System.Xml;
    using System.ServiceModel.Channels;

    /// <summary>
    /// The non SSL authentication http transport binding element.
    /// </summary>
    public class NonSslAuthHttpTransportBindingElement : HttpTransportBindingElement, ITransportTokenAssertionProvider
    {
        /// <summary>
        /// The clone.
        /// </summary>
        /// <returns>
        /// The <see cref="System.ServiceModel.Channels.BindingElement"/>.
        /// </returns>
        public override BindingElement Clone()
        {
            return new NonSslAuthHttpTransportBindingElement
            {
                AuthenticationScheme = AuthenticationScheme,
                ManualAddressing = ManualAddressing
            };
        }

        public XmlElement GetTransportTokenAssertion()
        {
            return null;
        }

        /// <summary>
        /// The get property.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <typeparam name="T">
        /// ISecurityCapabilities type
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public override T GetProperty<T>(BindingContext context)
        {
            if (typeof(T) == typeof(ISecurityCapabilities))
            {
                return (T)(object)new NonSslAuthSecurityCapabilities();
            }
            return base.GetProperty<T>(context);
        }
    }
}
