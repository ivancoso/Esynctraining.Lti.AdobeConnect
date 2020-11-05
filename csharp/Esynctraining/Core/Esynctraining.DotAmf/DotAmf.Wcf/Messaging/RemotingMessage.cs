// --------------------------------------------------------------------------------------------------------------------
// <copyright company="" file="RemotingMessage.cs">
//   
// </copyright>
// <summary>
//   RemotingMessages are used to send RPC requests to a remote endpoint.
//   These messages use the operation property to specify which method to call
//   on the remote object. The destination property indicates what object/service
//   should be used.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace DotAmf.ServiceModel.Messaging
{
    using System.Runtime.Serialization;

    /// <summary>
    ///     RemotingMessages are used to send RPC requests to a remote endpoint.
    ///     These messages use the operation property to specify which method to call
    ///     on the remote object. The destination property indicates what object/service
    ///     should be used.
    /// </summary>
    [DataContract(Name = "flex.messaging.messages.RemotingMessage")]
    public sealed class RemotingMessage : AbstractMessage
    {
        #region Public Properties

        /// <summary>
        ///     Provides access to the name of the remote method/operation
        ///     that should be called.
        /// </summary>
        [DataMember(Name = "operation")]
        public string Operation { get; set; }

        #endregion
    }
}