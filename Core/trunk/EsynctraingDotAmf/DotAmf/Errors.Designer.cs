﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34014
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DotAmf {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Errors {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Errors() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("DotAmf.Errors", typeof(Errors).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Switching current AMF context to {0}..
        /// </summary>
        internal static string AbstractAmfDecoder_CurrentAmfVersion_Debug {
            get {
                return ResourceManager.GetString("AbstractAmfDecoder_CurrentAmfVersion_Debug", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to AMF version is not supported: {0}.
        /// </summary>
        internal static string Amf0Decoder_ReadAmfValue_AmfVersionNotSupported {
            get {
                return ResourceManager.GetString("Amf0Decoder_ReadAmfValue_AmfVersionNotSupported", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Value type marker not found..
        /// </summary>
        internal static string Amf0Decoder_ReadAmfValue_TypeMarkerMissing {
            get {
                return ResourceManager.GetString("Amf0Decoder_ReadAmfValue_TypeMarkerMissing", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid array length..
        /// </summary>
        internal static string Amf0Decoder_ReadEcmaArray_InvalidLength {
            get {
                return ResourceManager.GetString("Amf0Decoder_ReadEcmaArray_InvalidLength", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Found {0} message headers..
        /// </summary>
        internal static string Amf0Decoder_ReadPacketHeaders_Debug {
            get {
                return ResourceManager.GetString("Amf0Decoder_ReadPacketHeaders_Debug", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Found {0} message bodies..
        /// </summary>
        internal static string Amf0Decoder_ReadPacketMessages_Debug {
            get {
                return ResourceManager.GetString("Amf0Decoder_ReadPacketMessages_Debug", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unable to deserialize properties map..
        /// </summary>
        internal static string Amf0Decoder_ReadPropertiesMap_UnableToDeserialize {
            get {
                return ResourceManager.GetString("Amf0Decoder_ReadPropertiesMap_UnableToDeserialize", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unexpected object end..
        /// </summary>
        internal static string Amf0Decoder_ReadPropertiesMap_UnexpectedObjectEnd {
            get {
                return ResourceManager.GetString("Amf0Decoder_ReadPropertiesMap_UnexpectedObjectEnd", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Referenced object #{0} not found..
        /// </summary>
        internal static string Amf0Decoder_ReadReference_BadIndex {
            get {
                return ResourceManager.GetString("Amf0Decoder_ReadReference_BadIndex", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unknown type with type marker &apos;{0}&apos;..
        /// </summary>
        internal static string Amf0Decoder_ReadValue_UnknownType {
            get {
                return ResourceManager.GetString("Amf0Decoder_ReadValue_UnknownType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to AMF header has invalid format..
        /// </summary>
        internal static string Amf0Deserializer_ReadPacketHeaders_InvalidFormat {
            get {
                return ResourceManager.GetString("Amf0Deserializer_ReadPacketHeaders_InvalidFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to AMF message has invalid format..
        /// </summary>
        internal static string Amf0Deserializer_ReadPacketMessages_InvalidFormat {
            get {
                return ResourceManager.GetString("Amf0Deserializer_ReadPacketMessages_InvalidFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Type &apos;{0}&apos; is not supported..
        /// </summary>
        internal static string Amf0Deserializer_ReadValue_UnsupportedType {
            get {
                return ResourceManager.GetString("Amf0Deserializer_ReadValue_UnsupportedType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Type is dynamic..
        /// </summary>
        internal static string Amf3Decoder_ReadObject_Debug_Dynamic {
            get {
                return ResourceManager.GetString("Amf3Decoder_ReadObject_Debug_Dynamic", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Dynamic portion of the type has ended..
        /// </summary>
        internal static string Amf3Decoder_ReadObject_Debug_DynamicEnd {
            get {
                return ResourceManager.GetString("Amf3Decoder_ReadObject_Debug_DynamicEnd", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Type has been read successfully..
        /// </summary>
        internal static string Amf3Decoder_ReadObject_Debug_End {
            get {
                return ResourceManager.GetString("Amf3Decoder_ReadObject_Debug_End", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Type is externizable..
        /// </summary>
        internal static string Amf3Decoder_ReadObject_Debug_Externizable {
            get {
                return ResourceManager.GetString("Amf3Decoder_ReadObject_Debug_Externizable", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Type contains {0} members..
        /// </summary>
        internal static string Amf3Decoder_ReadObject_Debug_Members {
            get {
                return ResourceManager.GetString("Amf3Decoder_ReadObject_Debug_Members", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Found type traits for type &apos;{0}&apos;..
        /// </summary>
        internal static string Amf3Decoder_ReadObject_Debug_Name {
            get {
                return ResourceManager.GetString("Amf3Decoder_ReadObject_Debug_Name", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Reading dynamic type members..
        /// </summary>
        internal static string Amf3Decoder_ReadObject_Debug_ReadingDynamic {
            get {
                return ResourceManager.GetString("Amf3Decoder_ReadObject_Debug_ReadingDynamic", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Trying to read a value for a dynamic member &apos;{0}&apos;..
        /// </summary>
        internal static string Amf3Decoder_ReadObject_Debug_ReadingDynamicField {
            get {
                return ResourceManager.GetString("Amf3Decoder_ReadObject_Debug_ReadingDynamicField", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Trying to read a value for a member #{0} &apos;{1}&apos;..
        /// </summary>
        internal static string Amf3Decoder_ReadObject_Debug_ReadingField {
            get {
                return ResourceManager.GetString("Amf3Decoder_ReadObject_Debug_ReadingField", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Reading value of type {0} from position {1}..
        /// </summary>
        internal static string Amf3Decoder_ReadValue_Debug {
            get {
                return ResourceManager.GetString("Amf3Decoder_ReadValue_Debug", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Finished reading value of type {0} on position {1}..
        /// </summary>
        internal static string Amf3Decoder_ReadValue_End {
            get {
                return ResourceManager.GetString("Amf3Decoder_ReadValue_End", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Type marker not found at position {0}..
        /// </summary>
        internal static string Amf3Decoder_ReadValue_InvalidMarker {
            get {
                return ResourceManager.GetString("Amf3Decoder_ReadValue_InvalidMarker", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Value type marker not found at position {0}..
        /// </summary>
        internal static string Amf3Decoder_ReadValue_TypeMarkerNotFound {
            get {
                return ResourceManager.GetString("Amf3Decoder_ReadValue_TypeMarkerNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Error instantiating a data contract object..
        /// </summary>
        internal static string Amf3Deserializer_ReadObject_InstantiationError {
            get {
                return ResourceManager.GetString("Amf3Deserializer_ReadObject_InstantiationError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Length cannot be negative..
        /// </summary>
        internal static string Amf3Deserializer_ReadString_NegativeLength {
            get {
                return ResourceManager.GetString("Amf3Deserializer_ReadString_NegativeLength", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Alias &apos;{0}&apos; is already taken by another type..
        /// </summary>
        internal static string AmfContractResolver_AliasCollision {
            get {
                return ResourceManager.GetString("AmfContractResolver_AliasCollision", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid alias name..
        /// </summary>
        internal static string AmfContractResolver_InvalidAliasName {
            get {
                return ResourceManager.GetString("AmfContractResolver_InvalidAliasName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unable to register type &apos;{0}&apos;..
        /// </summary>
        internal static string AmfContractResolver_TypeRegistrationError {
            get {
                return ResourceManager.GetString("AmfContractResolver_TypeRegistrationError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid type name..
        /// </summary>
        internal static string AmfDataContractResolver_ProxyInvalidTypeName {
            get {
                return ResourceManager.GetString("AmfDataContractResolver_ProxyInvalidTypeName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Proxy type with the given type name already exists..
        /// </summary>
        internal static string AmfDataContractResolver_ProxyTypeAlreadyExists {
            get {
                return ResourceManager.GetString("AmfDataContractResolver_ProxyTypeAlreadyExists", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Contract is not registered..
        /// </summary>
        internal static string AmfDataContractResolver_TryResolveTypeContracNotRegistered {
            get {
                return ResourceManager.GetString("AmfDataContractResolver_TryResolveTypeContracNotRegistered", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Error during serialization. Check inner exception for details..
        /// </summary>
        internal static string AmfPacketEncoder_EncodingError {
            get {
                return ResourceManager.GetString("AmfPacketEncoder_EncodingError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Context switch is required for reading data from AMF packets..
        /// </summary>
        internal static string AmfPacketReader_AmfPacketReader_ContextSwitchRequired {
            get {
                return ResourceManager.GetString("AmfPacketReader_AmfPacketReader_ContextSwitchRequired", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Error during AMF packet decoding. Check inner exception for details..
        /// </summary>
        internal static string AmfPacketReader_DecodingError {
            get {
                return ResourceManager.GetString("AmfPacketReader_DecodingError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unable to read from the stream..
        /// </summary>
        internal static string AmfPacketReader_Read_StreamClosed {
            get {
                return ResourceManager.GetString("AmfPacketReader_Read_StreamClosed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unable to read AMF version. Data has unknown format..
        /// </summary>
        internal static string AmfPacketReader_ReadPacketVersion_VersionReadError {
            get {
                return ResourceManager.GetString("AmfPacketReader_ReadPacketVersion_VersionReadError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Packet contains no messages..
        /// </summary>
        internal static string AmfPacketWriter_Write_PacketEmpty {
            get {
                return ResourceManager.GetString("AmfPacketWriter_Write_PacketEmpty", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unable to write to the stream..
        /// </summary>
        internal static string AmfPacketWriter_Write_StreamNotWriteable {
            get {
                return ResourceManager.GetString("AmfPacketWriter_Write_StreamNotWriteable", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Context switch makes sence only in AMF3..
        /// </summary>
        internal static string AmfSerializationContext_AmfSerializationContext_IllegalContextSwitch {
            get {
                return ResourceManager.GetString("AmfSerializationContext_AmfSerializationContext_IllegalContextSwitch", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unable to deserialize data contracts..
        /// </summary>
        internal static string DataContractAmfSerializer_ReadObject_ContractsError {
            get {
                return ResourceManager.GetString("DataContractAmfSerializer_ReadObject_ContractsError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Error reading AMF data..
        /// </summary>
        internal static string DataContractAmfSerializer_ReadObject_ErrorReadingAmf {
            get {
                return ResourceManager.GetString("DataContractAmfSerializer_ReadObject_ErrorReadingAmf", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Stream does not allow reading or seeking..
        /// </summary>
        internal static string DataContractAmfSerializer_ReadObject_InvalidStream {
            get {
                return ResourceManager.GetString("DataContractAmfSerializer_ReadObject_InvalidStream", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unable to serialize data contracts..
        /// </summary>
        internal static string DataContractAmfSerializer_WriteObject_ContractsError {
            get {
                return ResourceManager.GetString("DataContractAmfSerializer_WriteObject_ContractsError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Error writing AMF data..
        /// </summary>
        internal static string DataContractAmfSerializer_WriteObject_ErrorWritingAmf {
            get {
                return ResourceManager.GetString("DataContractAmfSerializer_WriteObject_ErrorWritingAmf", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Stream does not allow writing..
        /// </summary>
        internal static string DataContractAmfSerializer_WriteObject_InvalidStream {
            get {
                return ResourceManager.GetString("DataContractAmfSerializer_WriteObject_InvalidStream", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Type &apos;{0}&apos; is not a valid AMF data contract..
        /// </summary>
        internal static string DataContractUtil_GetContractAlias_InvalidContract {
            get {
                return ResourceManager.GetString("DataContractUtil_GetContractAlias_InvalidContract", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unable to set undefined property {0} on not dynamic object..
        /// </summary>
        internal static string SettingMissingProperty {
            get {
                return ResourceManager.GetString("SettingMissingProperty", resourceCulture);
            }
        }
    }
}
