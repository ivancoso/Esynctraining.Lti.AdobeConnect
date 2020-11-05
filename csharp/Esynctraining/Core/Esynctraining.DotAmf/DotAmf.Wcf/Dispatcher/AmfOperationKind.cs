// --------------------------------------------------------------------------------------------------------------------
// <copyright company="" file="AmfOperationKind.cs">
//   
// </copyright>
// <summary>
//   AMF operation kinds.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace DotAmf.ServiceModel.Dispatcher
{
    /// <summary>
    ///     AMF operation kinds.
    /// </summary>
    internal static class AmfOperationKind
    {
        #region Constants

        /// <summary>
        ///     Batch operation.
        /// </summary>
        public const string Batch = "@AmfBatchOperation";

        /// <summary>
        ///     Command.
        /// </summary>
        public const string Command = "@AmfCommand";

        /// <summary>
        ///     Fault.
        /// </summary>
        public const string Fault = "@AmfFault";

        #endregion
    }
}