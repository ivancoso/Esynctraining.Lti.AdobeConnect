// --------------------------------------------------------------------------------------------------------------------
// <copyright company="" file="AmfPacket.cs">
//   
// </copyright>
// <summary>
//   AMF packet.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace DotAmf.Data
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    ///     AMF packet.
    /// </summary>
    [DataContract(Name = "#AmfPacket", Namespace = "http://dotamf.net/")]
    public sealed class AmfPacket
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="AmfPacket" /> class.
        /// </summary>
        public AmfPacket()
        {
            this.Headers = new Dictionary<string, AmfHeader>();
            this.Messages = new List<AmfMessage>();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Packet headers.
        /// </summary>
        public IDictionary<string, AmfHeader> Headers { get; private set; }

        /// <summary>
        ///     Packet messages.
        /// </summary>
        public IList<AmfMessage> Messages { get; private set; }

        #endregion
    }
}