// --------------------------------------------------------------------------------------------------------------------
// <copyright company="" file="AcknowledgeMessage.cs">
//   
// </copyright>
// <summary>
//   An AcknowledgeMessage acknowledges the receipt of a message that was sent previously.
//   Every message sent within the messaging system must receive an acknowledgement.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace DotAmf.ServiceModel.Messaging
{
    using System.Runtime.Serialization;

    /// <summary>
    ///     An AcknowledgeMessage acknowledges the receipt of a message that was sent previously.
    ///     Every message sent within the messaging system must receive an acknowledgement.
    /// </summary>
    [DataContract(Name = "flex.messaging.messages.AcknowledgeMessage")]
    public class AcknowledgeMessage : AbstractMessage
    {
    }
}