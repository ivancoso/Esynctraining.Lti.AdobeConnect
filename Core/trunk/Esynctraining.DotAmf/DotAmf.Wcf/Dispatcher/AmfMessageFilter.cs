// --------------------------------------------------------------------------------------------------------------------
// <copyright company="" file="AmfMessageFilter.cs">
//   
// </copyright>
// <summary>
//   AMF message filter.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace DotAmf.ServiceModel.Dispatcher
{
    using System.ServiceModel.Channels;
    using System.ServiceModel.Dispatcher;

    /// <summary>
    ///     AMF message filter.
    /// </summary>
    internal sealed class AmfMessageFilter : MessageFilter
    {
        // All messages will be filtered in AmfDispatchOperationSelector
        // since there is no way to check if message is valid before
        // deserializing it.
        #region Public Methods and Operators

        /// <summary>
        /// The match.
        /// </summary>
        /// <param name="buffer">
        /// The buffer.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public override bool Match(MessageBuffer buffer)
        {
            return true;
        }

        /// <summary>
        /// The match.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public override bool Match(Message message)
        {
            return true;
        }

        #endregion
    }
}