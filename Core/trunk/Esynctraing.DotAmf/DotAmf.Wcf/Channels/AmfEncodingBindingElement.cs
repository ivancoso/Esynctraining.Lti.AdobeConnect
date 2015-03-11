// --------------------------------------------------------------------------------------------------------------------
// <copyright company="" file="AmfEncodingBindingElement.cs">
//   
// </copyright>
// <summary>
//   The binding element that sets the message version used to encode messages to AMF.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace DotAmf.ServiceModel.Channels
{
    using System;
    using System.ServiceModel.Channels;

    using DotAmf.Data;

    /// <summary>
    ///     The binding element that sets the message version used to encode messages to AMF.
    /// </summary>
    public sealed class AmfEncodingBindingElement : MessageEncodingBindingElement
    {
        #region Fields

        /// <summary>
        ///     AMF encoding options.
        /// </summary>
        private readonly AmfEncodingOptions _encodingOptions;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AmfEncodingBindingElement"/> class.
        ///     Constructor.
        /// </summary>
        /// <param name="encodingOptions">
        /// AMF encoding options.
        /// </param>
        public AmfEncodingBindingElement(AmfEncodingOptions encodingOptions)
        {
            this._encodingOptions = encodingOptions;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the message version that can be handled by the message encoders
        ///     produced by the message encoder factory.
        /// </summary>
        /// <remarks>Always set to <c>None</c> to make sure that no wrapping is applied.</remarks>
        public override MessageVersion MessageVersion
        {
            get
            {
                return MessageVersion.None;
            }

            set
            {
                throw new NotSupportedException();
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The build channel factory.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <typeparam name="TChannel">
        /// </typeparam>
        /// <returns>
        /// The <see cref="IChannelFactory"/>.
        /// </returns>
        public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context)
        {
            context.BindingParameters.Add(this);
            return base.BuildChannelFactory<TChannel>(context);
        }

        /// <summary>
        /// The build channel listener.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <typeparam name="TChannel">
        /// </typeparam>
        /// <returns>
        /// The <see cref="IChannelListener"/>.
        /// </returns>
        public override IChannelListener<TChannel> BuildChannelListener<TChannel>(BindingContext context)
        {
            context.BindingParameters.Add(this);
            return base.BuildChannelListener<TChannel>(context);
        }

        /// <summary>
        ///     Returns a copy of the binding element object.
        /// </summary>
        /// <returns>A BindingElement object that is a deep clone of the original.</returns>
        public override BindingElement Clone()
        {
            return new AmfEncodingBindingElement(this._encodingOptions);
        }

        /// <summary>
        ///     Creates a factory for producing message encoders.
        /// </summary>
        /// <returns>The <c>MessageEncoderFactory</c> used to produce message encoders.</returns>
        public override MessageEncoderFactory CreateMessageEncoderFactory()
        {
            return new AmfEncoderFactory(this._encodingOptions);
        }

        #endregion
    }
}