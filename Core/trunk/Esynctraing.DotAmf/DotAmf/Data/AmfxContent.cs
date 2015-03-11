// --------------------------------------------------------------------------------------------------------------------
// <copyright company="" file="AmfxContent.cs">
//   
// </copyright>
// <summary>
//   AMFX content.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace DotAmf.Data
{
    /// <summary>
    ///     AMFX content.
    /// </summary>
    internal static class AmfxContent
    {
        #region Constants

        /// <summary>
        ///     Root AMFX tag.
        /// </summary>
        public const string AmfxDocument = "amfx";

        /// <summary>
        ///     Array.
        /// </summary>
        public const string Array = "array";

        /// <summary>
        ///     ECMA array flag.
        /// </summary>
        public const string ArrayEcma = "ecma";

        /// <summary>
        ///     ECMA array item.
        /// </summary>
        public const string ArrayItem = "item";

        /// <summary>
        ///     ECMA array item's key.
        /// </summary>
        public const string ArrayKey = "name";

        /// <summary>
        ///     Array length.
        /// </summary>
        public const string ArrayLength = "length";

        /// <summary>
        ///     Boolean type.
        /// </summary>
        public const string Boolean = "bool";

        /// <summary>
        ///     Byte array value.
        /// </summary>
        public const string ByteArray = "bytes";

        /// <summary>
        ///     Date value.
        /// </summary>
        public const string Date = "date";

        /// <summary>
        ///     Floating number value.
        /// </summary>
        public const string Double = "double";

        /// <summary>
        ///     Boolean <c>false</c> value.
        /// </summary>
        public const string False = "false";

        /// <summary>
        ///     Integer value.
        /// </summary>
        public const string Integer = "int";

        /// <summary>
        ///     AMFX namespace.
        /// </summary>
        public const string Namespace = "http://www.macromedia.com/2005/amfx";

        /// <summary>
        ///     Null value.
        /// </summary>
        public const string Null = "null";

        /// <summary>
        ///     Object.
        /// </summary>
        public const string Object = "object";

        /// <summary>
        ///     Object's type.
        /// </summary>
        public const string ObjectType = "type";

        /// <summary>
        ///     Packet body.
        /// </summary>
        public const string PacketBody = "body";

        /// <summary>
        ///     Packet body count.
        /// </summary>
        public const string PacketBodyCount = "bodies";

        /// <summary>
        ///     Packet body response.
        /// </summary>
        public const string PacketBodyResponse = "responseURI";

        /// <summary>
        ///     Packet body target.
        /// </summary>
        public const string PacketBodyTarget = "targetURI";

        /// <summary>
        ///     Packet header.
        /// </summary>
        public const string PacketHeader = "header";

        /// <summary>
        ///     Packet header count.
        /// </summary>
        public const string PacketHeaderCount = "headers";

        /// <summary>
        ///     Packet header "must understand" flag.
        /// </summary>
        public const string PacketHeaderMustUnderstand = "mustUnderstand";

        /// <summary>
        ///     Packet header name.
        /// </summary>
        public const string PacketHeaderName = "name";

        /// <summary>
        ///     Object reference.
        /// </summary>
        public const string Reference = "ref";

        /// <summary>
        ///     Object reference ID.
        /// </summary>
        public const string ReferenceId = "id";

        /// <summary>
        ///     String value.
        /// </summary>
        public const string String = "string";

        /// <summary>
        ///     String reference ID.
        /// </summary>
        public const string StringId = "id";

        /// <summary>
        ///     Object's traits.
        /// </summary>
        public const string Traits = "traits";

        /// <summary>
        ///     Object's traits externizable flag.
        /// </summary>
        public const string TraitsExternizable = "externizable";

        /// <summary>
        ///     Object's traits reference ID.
        /// </summary>
        public const string TraitsId = "id";

        /// <summary>
        ///     Boolean <c>true</c> value.
        /// </summary>
        public const string True = "true";

        /// <summary>
        ///     AMFX AMF0 version value.
        /// </summary>
        public const string VersionAmf0 = "0";

        /// <summary>
        ///     AMFX AMF3 version value.
        /// </summary>
        public const string VersionAmf3 = "3";

        /// <summary>
        ///     AMFX version attribute.
        /// </summary>
        public const string VersionAttribute = "ver";

        /// <summary>
        ///     XML value.
        /// </summary>
        public const string Xml = "xml";

        #endregion
    }
}