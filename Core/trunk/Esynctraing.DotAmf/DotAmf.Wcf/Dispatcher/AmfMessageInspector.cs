// --------------------------------------------------------------------------------------------------------------------
// <copyright company="" file="AmfMessageInspector.cs">
//   
// </copyright>
// <summary>
//   AMF message inspector.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace DotAmf.ServiceModel.Dispatcher
{
    using System;
    using System.IO;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Dispatcher;
    using System.Xml;

    using DotAmf.Data;
    using DotAmf.IO;
    using DotAmf.ServiceModel.Channels;

    /// <summary>
    ///     AMF message inspector.
    /// </summary>
    internal sealed class AmfMessageInspector : IDispatchMessageInspector
    {
        #region Fields

        /// <summary>
        ///     Endpoint context.
        /// </summary>
        private readonly AmfEndpointContext _context;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AmfMessageInspector"/> class.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        public AmfMessageInspector(AmfEndpointContext context)
        {
            this._context = context;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The after receive request.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <param name="channel">
        /// The channel.
        /// </param>
        /// <param name="instanceContext">
        /// The instance context.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            return null;
        }

        /// <summary>
        /// The before send reply.
        /// </summary>
        /// <param name="reply">
        /// The reply.
        /// </param>
        /// <param name="correlationState">
        /// The correlation state.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// </exception>
        public void BeforeSendReply(ref Message reply, object correlationState)
        {
            var message = reply as AmfGenericMessage;

            if (message == null)
            {
                throw new InvalidOperationException("AmfGenericMessage is expected.");
            }

            var packet = new AmfPacket();

            foreach (var header in message.AmfHeaders)
            {
                packet.Headers[header.Key] = header.Value;
            }

            packet.Messages.Add(message.AmfMessage);

            var ms = new MemoryStream();

            try
            {
                // Serialize packet into AMFX data
                XmlWriter output = AmfxWriter.Create(ms);
                this._context.AmfSerializer.WriteObject(output, packet);
                output.Flush();
                ms.Seek(0, SeekOrigin.Begin);

                reply = Message.CreateMessage(MessageVersion.None, null, AmfxReader.Create(ms, true));
            }
            catch
            {
                ms.Dispose();
                throw;
            }
        }

        #endregion
    }
}