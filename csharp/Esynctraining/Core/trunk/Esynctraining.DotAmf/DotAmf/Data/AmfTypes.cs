// --------------------------------------------------------------------------------------------------------------------
// <copyright company="" file="AmfTypes.cs">
//   
// </copyright>
// <summary>
//   AMF object.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace DotAmf.Data
{
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Serialization;

    /// <summary>
    ///     AMF object.
    /// </summary>
    [DataContract(Namespace = "http://dotamf.net/")]
    public sealed class AmfObject : Dictionary<string, object>
    {
        #region Constants

        /// <summary>
        ///     Externizable data property name.
        /// </summary>
        public const string ExternizableProperty = "$IExternalizable";

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="AmfObject" /> class.
        /// </summary>
        public AmfObject()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AmfObject"/> class.
        /// </summary>
        /// <param name="dictionary">
        /// The dictionary.
        /// </param>
        public AmfObject(IDictionary<string, object> dictionary)
            : base(dictionary)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Type traits.
        /// </summary>
        [DataMember]
        public AmfTypeTraits Traits { get; set; }

        #endregion
    }

    /// <summary>
    ///     AMF+ type's traits.
    /// </summary>
    [DataContract(Namespace = "http://dotamf.net/")]
    public sealed class AmfTypeTraits
    {
        #region Constants

        /// <summary>
        ///     Base type alias.
        /// </summary>
        public const string BaseTypeAlias = "";

        #endregion

        #region Public Properties

        /// <summary>
        ///     A list of type members.
        /// </summary>
        [DataMember]
        public string[] ClassMembers { get; set; }

        /// <summary>
        ///     Type is dynamic.
        /// </summary>
        [DataMember]
        public bool IsDynamic { get; set; }

        /// <summary>
        ///     Type is externalizable.
        /// </summary>
        [DataMember]
        public bool IsExternalizable { get; set; }

        /// <summary>
        ///     Fully qualified type name.
        /// </summary>
        [DataMember]
        public string TypeName { get; set; }

        #endregion
    }

    /// <summary>
    ///     AMF externalizable data.
    /// </summary>
    [DataContract(Namespace = "http://dotamf.net/")]
    public sealed class AmfExternalizable
    {
        #region Public Properties

        /// <summary>
        ///     Data.
        /// </summary>
        [DataMember]
        public byte[] Data { get; set; }

        /// <summary>
        ///     Type name.
        /// </summary>
        public string TypeName { get; set; }

        #endregion
    }

    #region IExternizable

    /// <summary>
    ///     Provides control over serialization of a type
    ///     as it is encoded into a data stream.
    /// </summary>
    public interface IExternalizable
    {
        #region Public Properties

        /// <summary>
        ///     Type name.
        /// </summary>
        string TypeName { get; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Decode itself from a data stream.
        /// </summary>
        /// <param name="input">
        /// The input.
        /// </param>
        void ReadExternal(Stream input);

        /// <summary>
        /// Encode itself for a data stream.
        /// </summary>
        /// <param name="ouput">
        /// The ouput.
        /// </param>
        void WriteExternal(Stream ouput);

        #endregion
    }

    #endregion
}