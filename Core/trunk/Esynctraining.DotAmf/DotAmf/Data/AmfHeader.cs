// --------------------------------------------------------------------------------------------------------------------
// <copyright company="" file="AmfHeader.cs">
//   
// </copyright>
// <summary>
//   AMF header.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace DotAmf.Data
{
    /// <summary>
    ///     AMF header.
    /// </summary>
    public sealed class AmfHeader
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="AmfHeader" /> class.
        /// </summary>
        public AmfHeader()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AmfHeader"/> class.
        /// </summary>
        /// <param name="descriptor">
        /// The descriptor.
        /// </param>
        public AmfHeader(AmfHeaderDescriptor descriptor)
        {
            this.Name = descriptor.Name;
            this.MustUnderstand = descriptor.MustUnderstand;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     A data associated with the header.
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        ///     Client must understand this header or handle an error if he can't.
        /// </summary>
        public bool MustUnderstand { get; set; }

        /// <summary>
        ///     A remote operation or method to be invoked by this header.
        /// </summary>
        public string Name { get; set; }

        #endregion
    }

    /// <summary>
    ///     AMF header descriptor.
    /// </summary>
    public struct AmfHeaderDescriptor
    {
        #region Fields

        /// <summary>
        ///     Client must understand this header or handle an error if he can't.
        /// </summary>
        public bool MustUnderstand;

        /// <summary>
        ///     A remote operation or method to be invoked by this header.
        /// </summary>
        public string Name;

        #endregion
    }
}