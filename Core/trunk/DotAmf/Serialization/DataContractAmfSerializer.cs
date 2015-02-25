// --------------------------------------------------------------------------------------------------------------------
// <copyright company="" file="DataContractAmfSerializer.cs">
//   
// </copyright>
// <summary>
//   Serializes objects to the Action Message Format (AMF) and deserializes AMF data to objects.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace DotAmf.Serialization
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Xml;

    using DotAmf.Data;
    using DotAmf.Decoder;
    using DotAmf.Encoder;
    using DotAmf.IO;

    /// <summary>
    ///     Serializes objects to the Action Message Format (AMF) and deserializes AMF data to objects.
    /// </summary>
    /// <remarks>
    ///     This class is thread-safe and is expensive to instantiate.
    /// </remarks>
    public sealed class DataContractAmfSerializer : XmlObjectSerializer
    {
        #region Fields

        /// <summary>
        ///     AMF encoding options.
        /// </summary>
        private readonly AmfEncodingOptions _encodingOptions;

        /// <summary>
        ///     Contains the types that may be present in the object graph.
        /// </summary>
        private readonly Dictionary<string, DataContractDescriptor> _knownTypes;

        /// <summary>
        ///     A Type that specifies the type of the instances that is serialized or deserialized.
        /// </summary>
        private readonly KeyValuePair<string, DataContractDescriptor> _type;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the DataContractAmfSerializer class to serialize or deserialize an object of the
        ///     specified type.
        /// </summary>
        /// <param name="type">
        /// A Type that specifies the type of the instances that is serialized or deserialized.
        /// </param>
        /// <exception cref="InvalidDataContractException">
        /// At least one of the types being serialized or deserialized does not conform to data contract rules.
        ///     For example, the DataContractAttribute attribute has not been applied to the type.
        /// </exception>
        public DataContractAmfSerializer(Type type)
            : this(type, new List<Type>(), CreateDefaultOptions())
        {
        }

        /// <summary>
        /// Initializes a new instance of the DataContractAmfSerializer class to serialize or deserialize an object of the
        ///     specified type.
        /// </summary>
        /// <param name="type">
        /// A Type that specifies the type of the instances that is serialized or deserialized.
        /// </param>
        /// <param name="encodingOptions">
        /// AMF encoding options.
        /// </param>
        /// <exception cref="InvalidDataContractException">
        /// At least one of the types being serialized or deserialized does not conform to data contract rules.
        ///     For example, the DataContractAttribute attribute has not been applied to the type.
        /// </exception>
        public DataContractAmfSerializer(Type type, AmfEncodingOptions encodingOptions)
            : this(type, new List<Type>(), encodingOptions)
        {
        }

        /// <summary>
        /// Initializes a new instance of the DataContractAmfSerializer class to serialize or deserialize an object of the
        ///     specified type,
        ///     and a collection of known types that may be present in the object graph.
        /// </summary>
        /// <param name="type">
        /// A Type that specifies the type of the instances that is serialized or deserialized.
        /// </param>
        /// <param name="knownTypes">
        /// An IEnumerable of Type that contains the types that may be present in the object graph.
        /// </param>
        /// <exception cref="InvalidDataContractException">
        /// At least one of the types being serialized or deserialized does not conform to data contract rules.
        ///     For example, the DataContractAttribute attribute has not been applied to the type.
        /// </exception>
        public DataContractAmfSerializer(Type type, IEnumerable<Type> knownTypes)
            : this(type, knownTypes, CreateDefaultOptions())
        {
        }

        /// <summary>
        /// Initializes a new instance of the DataContractAmfSerializer class to serialize or deserialize an object of the
        ///     specified type,
        ///     and a collection of known types that may be present in the object graph.
        /// </summary>
        /// <param name="type">
        /// A Type that specifies the type of the instances that is serialized or deserialized.
        /// </param>
        /// <param name="knownTypes">
        /// An IEnumerable of Type that contains the types that may be present in the object graph.
        /// </param>
        /// <param name="encodingOptions">
        /// AMF encoding options.
        /// </param>
        /// <exception cref="InvalidDataContractException">
        /// At least one of the types being serialized or deserialized does not conform to data contract rules.
        ///     For example, the DataContractAttribute attribute has not been applied to the type.
        /// </exception>
        public DataContractAmfSerializer(Type type, IEnumerable<Type> knownTypes, AmfEncodingOptions encodingOptions)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            this._type = PrepareDataContract(type);

            if (knownTypes == null)
            {
                throw new ArgumentNullException("knownTypes");
            }

            this._knownTypes = PrepareDataContracts(knownTypes);
            this._knownTypes[this._type.Key] = this._type.Value;

            this._encodingOptions = encodingOptions;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Gets a value that specifies whether the XmlDictionaryReader is positioned over
        ///     an AMFX element that represents an object the serializer can deserialize from.
        /// </summary>
        /// <param name="reader">
        /// The XmlDictionaryReader used to read the AMFX stream mapped from AMF.
        /// </param>
        /// <returns>
        /// <c>true</c> if the reader is positioned correctly; otherwise, <c>false</c>.
        /// </returns>
        public override bool IsStartObject(XmlDictionaryReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            return this.IsStartObject((XmlReader)reader);
        }

        /// <summary>
        /// Gets a value that specifies whether the XmlReader is positioned over
        ///     an AMFX element that represents an object the serializer can deserialize from.
        /// </summary>
        /// <param name="reader">
        /// The XmlReader used to read the AMFX stream mapped from AMF.
        /// </param>
        /// <returns>
        /// <c>true</c> if the reader is positioned correctly; otherwise, <c>false</c>.
        /// </returns>
        public override bool IsStartObject(XmlReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            return reader.NodeType == XmlNodeType.Element && reader.Name == this._type.Value.AmfxType;
        }

        /// <summary>
        /// Reads a document stream in the AMF (Action Message Format) format and returns the deserialized object.
        /// </summary>
        /// <param name="stream">
        /// The Stream to be read.
        /// </param>
        /// <returns>
        /// The deserialized object.
        /// </returns>
        /// <exception cref="SerializationException">
        /// Unable to resolve data contracts.
        /// </exception>
        public override object ReadObject(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            if (!stream.CanRead)
            {
                throw new ArgumentException(Errors.DataContractAmfSerializer_ReadObject_InvalidStream, "stream");
            }

            using (var buffer = new MemoryStream())
            {
                this.ReadObject(stream, AmfxWriter.Create(buffer));

                buffer.Position = 0;
                return this.ReadObject(new XmlTextReader(buffer));
            }
        }

        /// <summary>
        /// Reads a document stream in the AMF (Action Message Format) format and writes
        ///     it in the AMFX (Action Message Format in XML) format,
        /// </summary>
        /// <param name="stream">
        /// The Stream to be read.
        /// </param>
        /// <param name="output">
        /// AMFX writer.
        /// </param>
        /// <exception cref="SerializationException">
        /// Unable to resolve data contracts.
        /// </exception>
        public void ReadObject(Stream stream, XmlWriter output)
        {
            // Decode AMF packet
            if (this._type.Value.AmfxType == AmfxContent.AmfxDocument)
            {
                var decoder = new AmfPacketDecoder(this._encodingOptions);
                decoder.Decode(stream, output);
            }
                
                // Decode generic AMF data
            else
            {
                IAmfDecoder decoder = CreateDecoder(this._encodingOptions);
                decoder.Decode(stream, output);
            }
        }

        /// <summary>
        /// Reads the XML document mapped from AMFX (Action Message Format in XML)
        ///     with an XmlDictionaryReader and returns the deserialized object.
        /// </summary>
        /// <param name="reader">
        /// XML reader.
        /// </param>
        /// <returns>
        /// Object graph.
        /// </returns>
        /// <exception cref="SerializationException">
        /// Unable to resolve data contracts.
        /// </exception>
        public override object ReadObject(XmlDictionaryReader reader)
        {
            return this.ReadObject((XmlReader)reader);
        }

        /// <summary>
        /// Reads the AMFX document mapped from AMF with an XmlDictionaryReader and returns the deserialized object;
        ///     it also enables you to specify whether the serializer should verify that it is positioned on an appropriate
        ///     element before attempting to deserialize.
        /// </summary>
        /// <param name="reader">
        /// An XmlDictionaryReader used to read the AMFX document mapped from AMF.
        /// </param>
        /// <param name="verifyObjectName">
        /// <c>true</c> to check whether the enclosing XML element name and namespace
        ///     correspond to the expected name and namespace; otherwise, <c>false</c> to skip the verification.
        /// </param>
        /// <exception cref="SerializationException">
        /// Unable to resolve data contracts.
        /// </exception>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public override object ReadObject(XmlDictionaryReader reader, bool verifyObjectName)
        {
            return ReadObject((XmlReader)reader, verifyObjectName);
        }

        /// <summary>
        /// Reads the XML document mapped from AMFX (Action Message Format in XML)
        ///     with an XmlReader and returns the deserialized object.
        /// </summary>
        /// <param name="reader">
        /// XML reader.
        /// </param>
        /// <returns>
        /// Object graph.
        /// </returns>
        /// <exception cref="SerializationException">
        /// Unable to resolve data contracts.
        /// </exception>
        public override object ReadObject(XmlReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            var context = new SerializationContext(this._encodingOptions.AmfVersion) { KnownTypes = this._knownTypes };

            return this._type.Value.AmfxType == AmfxContent.AmfxDocument
                       ? DeserializePacket(reader, context) // Special case
                       : Deserialize(reader, context);
        }

        /// <summary>
        /// Reads the AMFX document mapped from AMF with an XmlReader and returns the deserialized object;
        ///     it also enables you to specify whether the serializer should verify that it is positioned on an appropriate
        ///     element before attempting to deserialize.
        /// </summary>
        /// <param name="reader">
        /// An XmlReader used to read the AMFX document mapped from AMF.
        /// </param>
        /// <param name="verifyObjectName">
        /// <c>true</c> to check whether the enclosing XML element name and namespace
        ///     correspond to the expected name and namespace; otherwise, <c>false</c> to skip the verification.
        /// </param>
        /// <exception cref="SerializationException">
        /// Unable to resolve data contracts.
        /// </exception>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public override object ReadObject(XmlReader reader, bool verifyObjectName)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            return verifyObjectName && !IsStartObject(reader) ? null : ReadObject(reader);
        }

        /// <summary>
        /// The write end object.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        public override void WriteEndObject(XmlDictionaryWriter writer)
        {
        }

        /// <summary>
        /// The write end object.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        public override void WriteEndObject(XmlWriter writer)
        {
        }

        /// <summary>
        /// Serializes a specified object to Action Message Format (AMF) data and writes the resulting AMF to a stream.
        /// </summary>
        /// <param name="stream">
        /// The Stream that is written to.
        /// </param>
        /// <param name="graph">
        /// The object that contains the data to write to the stream.
        /// </param>
        /// <exception cref="SerializationException">
        /// Unable to resolve data contracts.
        /// </exception>
        /// <exception cref="InvalidDataException">
        /// Error during encoding.
        /// </exception>
        public override void WriteObject(Stream stream, object graph)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            if (!stream.CanWrite)
            {
                throw new ArgumentException(Errors.DataContractAmfSerializer_WriteObject_InvalidStream, "stream");
            }

            using (var buffer = new MemoryStream())
            {
                XmlWriter writer = AmfxWriter.Create(buffer);
                WriteObject(writer, graph);

                writer.Flush();
                buffer.Position = 0;

                XmlReader reader = AmfxReader.Create(buffer);
                reader.MoveToContent();

                this.WriteObject(stream, reader);
            }
        }

        /// <summary>
        /// Reads a specified Action Message Format in XML (AMFX) data and writes the resulting AMF data to a stream.
        /// </summary>
        /// <param name="stream">
        /// The Stream to be written.
        /// </param>
        /// <param name="input">
        /// AMFX reader.
        /// </param>
        /// <exception cref="SerializationException">
        /// Unable to resolve data contracts.
        /// </exception>
        /// <exception cref="InvalidDataException">
        /// Error during encoding.
        /// </exception>
        public void WriteObject(Stream stream, XmlReader input)
        {
            // Encode AMF packet
            if (this._type.Value.AmfxType == AmfxContent.AmfxDocument)
            {
                var encoder = new AmfPacketEncoder(this._encodingOptions);
                encoder.Encode(stream, input);
            }
                
                // Encode generic AMF data
            else
            {
                IAmfEncoder encoder = CreateEncoder(this._encodingOptions);
                encoder.Encode(stream, input);
            }
        }

        /// <summary>
        /// Serializes an object to XML that may be mapped to Action Message Format in XML (AMFX).
        ///     Writes all the object data, including the starting XML element, content,
        ///     and closing element, with an XmlWriter.
        /// </summary>
        /// <param name="writer">
        /// XML writer.
        /// </param>
        /// <param name="graph">
        /// Object graph.
        /// </param>
        /// <exception cref="SerializationException">
        /// Unable to serialize data contracts.
        /// </exception>
        public override void WriteObject(XmlWriter writer, object graph)
        {
            var context = new SerializationContext(this._encodingOptions.AmfVersion) { KnownTypes = this._knownTypes };

            if (this._type.Value.AmfxType == AmfxContent.AmfxDocument)
            {
                SerializePacket(writer, graph, context); // Special case
            }
            else
            {
                Serialize(writer, graph, context);
            }
        }

        /// <summary>
        /// Writes the XML content that can be mapped to Action Message Format (AMF) using an XmlDictionaryWriter.
        /// </summary>
        /// <param name="writer">
        /// The XmlDictionaryWriter used to write to.
        /// </param>
        /// <param name="graph">
        /// The object to write.
        /// </param>
        public override void WriteObjectContent(XmlDictionaryWriter writer, object graph)
        {
            this.WriteObjectContent((XmlWriter)writer, graph);
        }

        /// <summary>
        /// Writes the XML content that can be mapped to Action Message Format (AMF) using an XmlWriter.
        /// </summary>
        /// <param name="writer">
        /// The XmlWriter used to write to.
        /// </param>
        /// <param name="graph">
        /// The object to write.
        /// </param>
        public override void WriteObjectContent(XmlWriter writer, object graph)
        {
            WriteObject(writer, graph);
        }

        /// <summary>
        /// The write start object.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        /// <param name="graph">
        /// The graph.
        /// </param>
        public override void WriteStartObject(XmlDictionaryWriter writer, object graph)
        {
        }

        /// <summary>
        /// The write start object.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        /// <param name="graph">
        /// The graph.
        /// </param>
        public override void WriteStartObject(XmlWriter writer, object graph)
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// The convert array.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="object[]"/>.
        /// </returns>
        private static object[] ConvertArray(object value)
        {
            if (value != null)
            {
                if (value is Array)
                {
                    try
                    {
                        var objArr = (Array)value;
                        var arr = new object[objArr.Length];
                        Array.Copy(objArr, arr, objArr.Length);
                        return arr;
                    }
                    catch (Exception)
                    {
                        return (object[])value;
                    }
                }

                if (value as IEnumerable != null)
                {
                    var enumerable = (IEnumerable)value;
                    return ConvertArray(enumerable.Cast<object>().ToArray());
                }
            }

            return new object[] { };
        }

        /// <summary>
        /// The convert byte array.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="byte"/>.
        /// </returns>
        private static byte[] ConvertByteArray(object value)
        {
            if (value != null)
            {
                if (value is Array)
                {
                    try
                    {
                        var objArr = (Array)value;
                        var arr = new byte[objArr.Length];
                        Array.Copy(objArr, arr, objArr.Length);
                        return arr;
                    }
                    catch (Exception)
                    {
                        return (byte[])value;
                    }
                }

                if (value as IEnumerable != null)
                {
                    var enumerable = (IEnumerable)value;
                    return ConvertByteArray(enumerable.Cast<byte>().ToArray());
                }
            }

            return new byte[] { };
        }

        /// <summary>
        /// Create AMF decoder.
        /// </summary>
        /// <param name="encodingOptions">
        /// AMF decoding options.
        /// </param>
        /// <returns>
        /// The <see cref="IAmfDecoder"/>.
        /// </returns>
        private static IAmfDecoder CreateDecoder(AmfEncodingOptions encodingOptions)
        {
            switch (encodingOptions.AmfVersion)
            {
                case AmfVersion.Amf0:
                    return new Amf0Decoder(encodingOptions);

                case AmfVersion.Amf3:
                    return new Amf3Decoder(encodingOptions);

                default:
                    throw new NotSupportedException();
            }
        }

        /// <summary>
        ///     Create default AMF encoding options.
        /// </summary>
        /// <returns>
        ///     The <see cref="AmfEncodingOptions" />.
        /// </returns>
        private static AmfEncodingOptions CreateDefaultOptions()
        {
            return new AmfEncodingOptions { AmfVersion = AmfVersion.Amf3, UseContextSwitch = false };
        }

        /// <summary>
        /// Create AMF encoder.
        /// </summary>
        /// <param name="encodingOptions">
        /// AMF encoding options.
        /// </param>
        /// <returns>
        /// The <see cref="IAmfEncoder"/>.
        /// </returns>
        private static IAmfEncoder CreateEncoder(AmfEncodingOptions encodingOptions)
        {
            switch (encodingOptions.AmfVersion)
            {
                case AmfVersion.Amf0:
                    return new Amf0Encoder(encodingOptions);

                case AmfVersion.Amf3:
                    return new Amf3Encoder(encodingOptions);

                default:
                    throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Deserialize AMFX data.
        /// </summary>
        /// <param name="reader">
        /// The reader.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <exception cref="SerializationException">
        /// Error during deserialization.
        /// </exception>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        private static object Deserialize(XmlReader reader, SerializationContext context)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (reader.NodeType != XmlNodeType.Element)
            {
                throw new XmlException(string.Format("Element node expected, {0} found.", reader.NodeType));
            }

            switch (reader.Name)
            {
                case AmfxContent.Null:
                    reader.Read();
                    return null;

                case AmfxContent.True:
                    reader.Read();
                    return true;

                case AmfxContent.False:
                    reader.Read();
                    return false;
            }

            

            object value;

            switch (reader.Name)
            {
                case AmfxContent.Integer:
                    value = ReadInteger(reader);
                    break;

                case AmfxContent.Double:
                    value = ReadDouble(reader);
                    break;

                case AmfxContent.String:
                    value = ReadString(reader, context);
                    break;

                case AmfxContent.Array:
                    value = ReadArray(reader, context);
                    break;

                case AmfxContent.ByteArray:
                    value = ReadByteArray(reader, context);
                    break;

                case AmfxContent.Date:
                    value = ReadDate(reader, context);
                    break;

                case AmfxContent.Xml:
                    value = ReadXml(reader, context);
                    break;

                case AmfxContent.Object:
                    value = ReadObject(reader, context);
                    break;

                case AmfxContent.Reference:
                    value = ReadReference(reader, context);
                    reader.Read();
                    break;

                default:
                    throw new NotSupportedException("Unexpected AMFX type: " + reader.Name);
            }

            return value;

            
        }

        /// <summary>
        /// Deserialize an AMFX packet.
        /// </summary>
        /// <param name="reader">
        /// The reader.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <exception cref="SerializationException">
        /// Error during deserialization.
        /// </exception>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        private static object DeserializePacket(XmlReader reader, SerializationContext context)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            var packet = new AmfPacket();

            while (reader.Read())
            {
                if (reader.NodeType != XmlNodeType.Element)
                {
                    continue;
                }

                if (reader.Name == AmfxContent.PacketHeader)
                {
                    var header = new AmfHeader
                                     {
                                         Name = reader.GetAttribute(AmfxContent.PacketHeaderName), 
                                         MustUnderstand =
                                             reader.GetAttribute(AmfxContent.PacketHeaderMustUnderstand)
                                             == AmfxContent.True
                                     };

                    while (reader.Read())
                    {
                        // Skip until header content is found, if any
                        if (reader.NodeType != XmlNodeType.Element || reader.Name == AmfxContent.PacketHeader)
                        {
                            continue;
                        }

                        header.Data = Deserialize(reader, context);
                        break;
                    }

                    packet.Headers[header.Name] = header;
                    continue;
                }

                

                if (reader.Name == AmfxContent.PacketBody)
                {
                    var message = new AmfMessage
                                      {
                                          Target = reader.GetAttribute(AmfxContent.PacketBodyTarget), 
                                          Response = reader.GetAttribute(AmfxContent.PacketBodyResponse)
                                      };

                    while (reader.Read())
                    {
                        // Skip until body content is found, if any
                        if (reader.NodeType != XmlNodeType.Element || reader.Name == AmfxContent.PacketBody)
                        {
                            continue;
                        }

                        message.Data = Deserialize(reader, context);
                        break;
                    }

                    packet.Messages.Add(message);
                }

                
            }

            return packet;
        }

        /// <summary>
        /// Get AMFX type for a CLR type.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <param name="isDataContract">
        /// The is Data Contract.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string GetAmfxType(Type type, out bool isDataContract)
        {
            isDataContract = false;

            TypeCode typecode = Type.GetTypeCode(type);

            switch (typecode)
            {
                    // A boolean value
                case TypeCode.Boolean:
                    return AmfxContent.Boolean;

                    // A string
                case TypeCode.String:
                    return AmfxContent.String;

                    // A date
                case TypeCode.DateTime:
                    return AmfxContent.Date;

                case TypeCode.Empty:
                    return AmfxContent.Date;
            }

            // An enumeration
            if (type.IsEnum)
            {
                return AmfxContent.Integer;
            }

            // Check if type is a number
            bool isInteger;
            if (DataContractHelper.IsNumericType(type, typecode, out isInteger))
            {
                return isInteger ? AmfxContent.Integer : AmfxContent.Double;
            }

            // An array
            if (type.IsArray)
            {
                // || type.GetInterfaces().Any(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
                return type == typeof(byte[]) ? AmfxContent.ByteArray : AmfxContent.Array;
            }

            // An XML document
            if (type == typeof(XmlDocument))
            {
                return AmfxContent.Xml;
            }

            // A guid
            if (type == typeof(Guid))
            {
                return AmfxContent.String;
            }

            // A special case
            if (type == typeof(AmfPacket))
            {
                isDataContract = true;
                return AmfxContent.AmfxDocument;
            }

            // A dictionary
            if (type.IsGenericType && typeof(IDictionary).IsAssignableFrom(type))
            {
                return AmfxContent.Object;
            }

            if (type.IsGenericType
                && type.GetInterfaces()
                       .Any(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
            {
                Type typeOfT = type.GetGenericArguments()[0];
                return typeOfT == typeof(byte) ? AmfxContent.ByteArray : AmfxContent.Array;
            }

            // Probably a data contract
            isDataContract = true;
            return AmfxContent.Object;
        }

        /// <summary>
        /// Get a string representation of the enum value.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <exception cref="SerializationException">
        /// Enum is not registered.
        /// </exception>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        private static object GetEnumValue(SerializationContext context, Type type, object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            EnumDescriptor descriptor = context.GetEnumDescriptor(type);

            if (descriptor == null)
            {
                throw new SerializationException(
                    string.Format(
                        "Unable to resolve type '{0}'. Check if type was registered within the serializer.", 
                        type.FullName));
            }

            if (!descriptor.Values.ContainsKey(value))
            {
                throw new SerializationException(string.Format("Invalid enumeration value '{0}'.", value));
            }

            return descriptor.Values[value];
        }

        /// <summary>
        /// Prepare a data contract.
        /// </summary>
        /// <param name="type">
        /// The type to prepare.
        /// </param>
        /// <returns>
        /// An alias-contract pair.
        /// </returns>
        /// <exception cref="InvalidDataContractException">
        /// Type does not conform to data contract rules.
        ///     For example, the DataContractAttribute attribute has not been applied to the type.
        /// </exception>
        private static KeyValuePair<string, DataContractDescriptor> PrepareDataContract(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            try
            {
                bool isDataContract;
                string amfxType = GetAmfxType(type, out isDataContract);
                string alias = isDataContract ? DataContractHelper.GetContractAlias(type) : type.FullName;

                DataContractDescriptor descriptor;

                if (type.IsEnum)
                {
                    descriptor = new EnumDescriptor { Values = DataContractHelper.GetEnumValues(type) };
                }
                else
                {
                    descriptor = new DataContractDescriptor();
                }

                descriptor.Alias = alias;
                descriptor.Type = type;
                descriptor.IsPrimitive = !isDataContract;
                descriptor.AmfxType = amfxType;

                if (isDataContract)
                {
                    descriptor.FieldMap = DataContractHelper.GetContractFields(type);
                    descriptor.PropertyMap = DataContractHelper.GetContractProperties(type);
                }

                return new KeyValuePair<string, DataContractDescriptor>(alias, descriptor);
            }
            catch (Exception e)
            {
                throw new InvalidDataContractException(
                    string.Format("Type '{0}' is not a valid data contract.", type.FullName), 
                    e);
            }
        }

        /// <summary>
        /// Prepare data contracts.
        /// </summary>
        /// <param name="knownTypes">
        /// The types that may be present in the object graph.
        /// </param>
        /// <returns>
        /// An alias-contract dictionary object.
        /// </returns>
        /// <exception cref="InvalidDataContractException">
        /// At least one of the types does not conform to data contract rules.
        ///     For example, the DataContractAttribute attribute has not been applied to the type.
        /// </exception>
        private static Dictionary<string, DataContractDescriptor> PrepareDataContracts(IEnumerable<Type> knownTypes)
        {
            if (knownTypes == null)
            {
                throw new ArgumentNullException("knownTypes");
            }

            return knownTypes.Select(PrepareDataContract).ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        /// <summary>
        /// The read array.
        /// </summary>
        /// <param name="reader">
        /// The reader.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        private static object ReadArray(XmlReader reader, SerializationContext context)
        {
            int length = Convert.ToInt32(reader.GetAttribute(AmfxContent.ArrayLength));

            var result = new object[length];
            context.References.Add(new AmfReference { Reference = result, AmfxType = AmfxContent.Array });

            int referenceIndex = context.References.Count - 1;

            reader.Read();

            if (length == 0)
            {
                return result;
            }

            for (int i = 0; i < length; i++)
            {
                result[i] = Deserialize(reader, context);
                reader.Read();
            }

            Type[] valueTypes = result.Where(x => x != null).Select(x => x.GetType()).Distinct().ToArray();

            if (valueTypes.Length == 1)
            {
                Type elementType = valueTypes.First();
                Array convertedArray = Array.CreateInstance(elementType, result.Length);

                for (int i = 0; i < result.Length; i++)
                {
                    convertedArray.SetValue(result[i], i);
                }

                context.References[referenceIndex] = new AmfReference
                                                         {
                                                             Reference = convertedArray, 
                                                             AmfxType = AmfxContent.Array
                                                         };

                return convertedArray;
            }

            return result;
        }

        /// <summary>
        /// The read byte array.
        /// </summary>
        /// <param name="reader">
        /// The reader.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The <see cref="byte[]"/>.
        /// </returns>
        private static byte[] ReadByteArray(XmlReader reader, SerializationContext context)
        {
            string encoded = reader.ReadString();
            byte[] bytes = Convert.FromBase64String(encoded);

            context.References.Add(new AmfReference { Reference = bytes, AmfxType = AmfxContent.ByteArray });

            return bytes;
        }

        /// <summary>
        /// The read date.
        /// </summary>
        /// <param name="reader">
        /// The reader.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        private static DateTime ReadDate(XmlReader reader, SerializationContext context)
        {
            long milliseconds = Convert.ToInt64(reader.ReadString());
            var origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan offset = TimeSpan.FromMilliseconds(milliseconds);
            DateTime result = origin + offset;

            context.References.Add(new AmfReference { Reference = result, AmfxType = AmfxContent.Date });

            return result;
        }

        /// <summary>
        /// The read double.
        /// </summary>
        /// <param name="reader">
        /// The reader.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        private static double ReadDouble(XmlReader reader)
        {
            return Convert.ToDouble(reader.ReadString());
        }

        /// <summary>
        /// The read integer.
        /// </summary>
        /// <param name="reader">
        /// The reader.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        private static int ReadInteger(XmlReader reader)
        {
            return Convert.ToInt32(reader.ReadString());
        }

        /// <summary>
        /// The read object.
        /// </summary>
        /// <param name="reader">
        /// The reader.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        /// <exception cref="SerializationException">
        /// </exception>
        private static object ReadObject(XmlReader reader, SerializationContext context)
        {
            var properties = new Dictionary<string, object>();

            var proxy = new object();
            context.References.Add(new AmfReference { Reference = proxy, AmfxType = AmfxContent.Object });

            int referenceIndex = context.References.Count - 1;

            AmfTypeTraits traits;
            string typeName = string.Empty;

            if (reader.HasAttributes)
            {
                typeName = reader.GetAttribute(AmfxContent.ObjectType);
            }

            reader.Read();

            if (!reader.IsEmptyElement && reader.GetAttribute(AmfxContent.TraitsId) == null)
            {
                traits = new AmfTypeTraits { TypeName = typeName };
                context.TraitsReferences.Add(traits);

                var members = new List<string>();

                reader.Read();

                while (reader.NodeType != XmlNodeType.EndElement)
                {
                    members.Add(reader.ReadElementContentAsString());
                }

                traits.ClassMembers = members.ToArray();

                reader.Read();
            }
            else
            {
                int index = Convert.ToInt32(reader.GetAttribute(AmfxContent.TraitsId));
                traits = context.TraitsReferences[index];
                reader.Read();
                reader.Read();
            }

            if (traits == null)
            {
                throw new SerializationException("Object traits not found.");
            }

            

            for (int i = 0; i < traits.ClassMembers.Length; i++)
            {
                object memberValue = Deserialize(reader, context);
                string memberName = traits.ClassMembers[i];

                properties[memberName] = memberValue;
                reader.Read();
            }

            

            #region Instantiate type

            object result;

            if (!string.IsNullOrEmpty(traits.TypeName))
            {
                if (!context.KnownTypes.ContainsKey(traits.TypeName))
                {
                    throw new SerializationException(
                        string.Format("Unable to find data contract for type alias '{0}'.", traits.TypeName));
                }

                DataContractDescriptor typeDescriptor = context.KnownTypes[traits.TypeName];

                result = DataContractHelper.InstantiateContract(
                    typeDescriptor.Type, 
                    properties, 
                    typeDescriptor.PropertyMap, 
                    typeDescriptor.FieldMap);
            }
            else
            {
                result = properties;
            }

            context.References[referenceIndex] = new AmfReference { Reference = result, AmfxType = AmfxContent.Object };

            return result;

            #endregion
        }

        /// <summary>
        /// The read reference.
        /// </summary>
        /// <param name="reader">
        /// The reader.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        private static object ReadReference(XmlReader reader, SerializationContext context)
        {
            int index = Convert.ToInt32(reader.GetAttribute(AmfxContent.ReferenceId));
            return context.References[index + 1].Reference;
        }

        /// <summary>
        /// The read string.
        /// </summary>
        /// <param name="reader">
        /// The reader.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string ReadString(XmlReader reader, SerializationContext context)
        {
            if (reader.IsEmptyElement)
            {
                if (reader.AttributeCount > 0)
                {
                    int index = Convert.ToInt32(reader.GetAttribute(AmfxContent.StringId));
                    return context.StringReferences[index];
                }

                return string.Empty;
            }

            string text = reader.ReadString();
            context.StringReferences.Add(text);

            return text;
        }

        /// <summary>
        /// The read xml.
        /// </summary>
        /// <param name="reader">
        /// The reader.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The <see cref="XmlDocument"/>.
        /// </returns>
        private static XmlDocument ReadXml(XmlReader reader, SerializationContext context)
        {
            string text = reader.ReadString();
            var result = new XmlDocument();
            result.LoadXml(text);

            context.References.Add(new AmfReference { Reference = result, AmfxType = AmfxContent.Xml });

            return result;
        }

        /// <summary>
        /// Serialize AMFX data.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <exception cref="SerializationException">
        /// Error during serialization.
        /// </exception>
        private static void Serialize(XmlWriter writer, object value, SerializationContext context)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }

            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            // A null value
            if (value == null)
            {
                WriteEmptyElement(writer, AmfxContent.Null);
                writer.Flush();
                return;
            }

            bool isDataContract;
            Type type = value.GetType();
            string amfxtype = GetAmfxType(type, out isDataContract);

            // A data contract type
            if (amfxtype == AmfxContent.Object)
            {
                if (isDataContract)
                {
                    DataContractDescriptor descriptor = context.GetDescriptor(type);

                    if (descriptor == null)
                    {
                        throw new SerializationException(
                            string.Format(
                                "Unable to resolve type '{0}'. Check if type was registered within the serializer.", 
                                type.FullName));
                    }
                }

                WriteDataContract(writer, value, type, context, isDataContract);
                writer.Flush();
                return;
            }

            switch (amfxtype)
            {
                case AmfxContent.Boolean:
                    WriteEmptyElement(writer, (bool)value ? AmfxContent.True : AmfxContent.False);
                    break;

                case AmfxContent.Integer:
                    {
                        WriteElement(
                            writer, 
                            amfxtype, 
                            type.IsEnum ? GetEnumValue(context, type, value).ToString() : value.ToString());
                        break;
                    }

                case AmfxContent.Double:
                    WriteElement(writer, amfxtype, value.ToString());
                    break;

                case AmfxContent.String:
                    WriteString(writer, value.ToString(), context);
                    break;

                case AmfxContent.Date:
                    WriteDate(writer, (DateTime)value, context);
                    break;

                case AmfxContent.ByteArray:
                    WriteByteArray(writer, ConvertByteArray(value), context);
                    break;

                case AmfxContent.Xml:
                    WriteXml(writer, (XmlDocument)value, context);
                    break;

                case AmfxContent.Array:
                    WriteArray(writer, ConvertArray(value), context);
                    break;

                default:
                    throw new SerializationException(string.Format("Unable to serialize type '{0}'", type.FullName));
            }

            writer.Flush();
        }

        /// <summary>
        /// Serialize an AMFX packet.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        /// <param name="graph">
        /// The graph.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <exception cref="SerializationException">
        /// Error during serialization.
        /// </exception>
        private static void SerializePacket(XmlWriter writer, object graph, SerializationContext context)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }

            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (graph == null)
            {
                throw new ArgumentNullException("graph");
            }

            var packet = graph as AmfPacket;
            if (packet == null)
            {
                throw new SerializationException("Object is not an AmfPacket");
            }

            writer.WriteStartDocument();
            writer.WriteStartElement(AmfxContent.AmfxDocument, AmfxContent.Namespace);
            writer.WriteAttributeString(AmfxContent.VersionAttribute, context.AmfVersion.ToAmfxName());
            writer.WriteAttributeString(AmfxContent.PacketHeaderCount, packet.Headers.Count.ToString());
            writer.WriteAttributeString(AmfxContent.PacketBodyCount, packet.Messages.Count.ToString());

            // Write headers
            foreach (AmfHeader header in packet.Headers.Values)
            {
                writer.WriteStartElement(AmfxContent.PacketHeader);
                writer.WriteAttributeString(AmfxContent.PacketHeaderName, header.Name);
                writer.WriteAttributeString(AmfxContent.PacketHeaderMustUnderstand, header.MustUnderstand.ToString());
                Serialize(writer, header.Data, context);
                writer.WriteEndElement();

                context.ResetReferences();
            }

            // Write bodies
            foreach (AmfMessage body in packet.Messages)
            {
                writer.WriteStartElement(AmfxContent.PacketBody);
                writer.WriteAttributeString(AmfxContent.PacketBodyTarget, body.Target);
                writer.WriteAttributeString(AmfxContent.PacketBodyResponse, body.Response);
                Serialize(writer, body.Data, context);
                writer.WriteEndElement();

                context.ResetReferences();
            }

            writer.WriteEndElement();
            writer.WriteEndDocument();
        }

        /// <summary>
        /// Write an array.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        private static void WriteArray(XmlWriter writer, object[] value, SerializationContext context)
        {
            int index = context.References.IndexOf(value);

            // Write an array
            if (index == -1)
            {
                context.References.Add(new AmfReference { Reference = value, AmfxType = AmfxContent.Array });

                writer.WriteStartElement(AmfxContent.Array);
                writer.WriteAttributeString(AmfxContent.ArrayLength, value.Length.ToString());

                foreach (object item in value)
                {
                    Serialize(writer, item, context);
                }

                writer.WriteEndElement();
            }
                
                // Write an array reference. Only in AMF+
            else
            {
                var attributes = new Dictionary<string, string> { { AmfxContent.ReferenceId, index.ToString() } };
                WriteEmptyElement(writer, AmfxContent.Reference, attributes);
            }
        }

        /// <summary>
        /// Write a byte array.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        private static void WriteByteArray(XmlWriter writer, byte[] value, SerializationContext context)
        {
            int index;

            // Write a byte array
            if (context.AmfVersion != AmfVersion.Amf3 || (index = context.References.IndexOf(value)) == -1)
            {
                string data = Convert.ToBase64String(value);

                context.References.Add(new AmfReference { Reference = value, AmfxType = AmfxContent.ByteArray });
                WriteElement(writer, AmfxContent.ByteArray, data);
            }
                
                // Write a byte array reference. Only in AMF+
            else
            {
                var attributes = new Dictionary<string, string> { { AmfxContent.ReferenceId, index.ToString() } };
                WriteEmptyElement(writer, AmfxContent.Reference, attributes);
            }
        }

        /// <summary>
        /// Write an object.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        /// <param name="graph">
        /// The graph.
        /// </param>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="isDataContract">
        /// The is Data Contract.
        /// </param>
        private static void WriteDataContract(
            XmlWriter writer, 
            object graph, 
            Type type, 
            SerializationContext context, 
            bool isDataContract)
        {
            int index = context.References.IndexOf(graph);

            // Write object reference
            if (index != -1)
            {
                var attributes = new Dictionary<string, string> { { AmfxContent.ReferenceId, index.ToString() } };
                WriteEmptyElement(writer, AmfxContent.Reference, attributes);
                return;
            }

            context.References.Add(new AmfReference { Reference = graph, AmfxType = AmfxContent.Object });

            Dictionary<string, object> properties;

            if (isDataContract)
            {
                DataContractDescriptor descriptor = context.GetDescriptor(type);

                if (descriptor == null)
                {
                    throw new SerializationException(
                        string.Format(
                            "Unable to resolve type '{0}'. Check if type was registered within the serializer.", 
                            type.FullName));
                }

                string alias = descriptor.Alias;
                int traitsindex = context.TraitsIndex(alias);

                writer.WriteStartElement(AmfxContent.Object);

                if (!string.IsNullOrEmpty(alias))
                {
                    writer.WriteAttributeString(AmfxContent.ObjectType, alias);

                    if (traitsindex == -1)
                    {
                        int typeNameIndex = context.StringReferences.IndexOf(alias);
                        if (typeNameIndex == -1)
                        {
                            context.StringReferences.Add(alias);
                        }
                    }
                }

                properties = DataContractHelper.GetContractProperties(
                    graph, 
                    descriptor.PropertyMap, 
                    descriptor.FieldMap);

                // Write traits by reference
                if (context.AmfVersion == AmfVersion.Amf3 && traitsindex != -1)
                {
                    var attributes = new Dictionary<string, string> { { AmfxContent.TraitsId, traitsindex.ToString() } };
                    WriteEmptyElement(writer, AmfxContent.Traits, attributes);
                }
                    
                    // Write traits
                else
                {
                    var traits = new AmfTypeTraits { TypeName = alias, ClassMembers = properties.Keys.ToArray() };
                    context.TraitsReferences.Add(traits);

                    writer.WriteStartElement(AmfxContent.Traits);

                    foreach (string propertyName in properties.Keys)
                    {
                        WriteElement(writer, AmfxContent.String, propertyName);

                        int memberNameIndex = context.StringReferences.IndexOf(propertyName);
                        if (memberNameIndex == -1)
                        {
                            context.StringReferences.Add(propertyName);
                        }
                    }

                    writer.WriteEndElement(); // End of traits
                }
            }
            else
            {
                var map = (IDictionary)graph;
                properties = new Dictionary<string, object>();

                foreach (object key in map.Keys)
                {
                    properties[key.ToString()] = map[key];
                }

                writer.WriteStartElement(AmfxContent.Object);

                writer.WriteStartElement(AmfxContent.Traits);

                foreach (string propertyName in properties.Keys)
                {
                    WriteElement(writer, AmfxContent.String, propertyName);
                    int memberNameIndex = context.StringReferences.IndexOf(propertyName);
                    if (memberNameIndex == -1)
                    {
                        context.StringReferences.Add(propertyName);
                    }
                }

                writer.WriteEndElement(); // End of traits
            }

            foreach (object value in properties.Values)
            {
                Serialize(writer, value, context);
            }

            writer.WriteEndElement(); // End of object
        }

        /// <summary>
        /// Write a date.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        private static void WriteDate(XmlWriter writer, DateTime value, SerializationContext context)
        {
            int index;

            // Write a date
            if (context.AmfVersion != AmfVersion.Amf3 || (index = context.References.IndexOf(value)) == -1)
            {
                double timestamp = DataContractHelper.ConvertToTimestamp(value);
                context.References.Add(new AmfReference { Reference = value, AmfxType = AmfxContent.Date });
                WriteElement(writer, AmfxContent.Date, timestamp.ToString());
            }
                
                // Write a date reference. Only in AMF+
            else
            {
                var attributes = new Dictionary<string, string> { { AmfxContent.ReferenceId, index.ToString() } };
                WriteEmptyElement(writer, AmfxContent.Reference, attributes);
            }
        }

        /// <summary>
        /// Write an element.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        /// <param name="elementName">
        /// The element Name.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="attributes">
        /// The attributes.
        /// </param>
        private static void WriteElement(
            XmlWriter writer, 
            string elementName, 
            string value, 
            IEnumerable<KeyValuePair<string, string>> attributes = null)
        {
            writer.WriteStartElement(elementName);

            if (attributes != null)
            {
                foreach (var pair in attributes)
                {
                    writer.WriteAttributeString(pair.Key, pair.Value);
                }
            }

            writer.WriteValue(value);
            writer.WriteEndElement();
        }

        /// <summary>
        /// Write an empty element.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        /// <param name="elementName">
        /// The element Name.
        /// </param>
        /// <param name="attributes">
        /// The attributes.
        /// </param>
        private static void WriteEmptyElement(
            XmlWriter writer, 
            string elementName, 
            IEnumerable<KeyValuePair<string, string>> attributes = null)
        {
            writer.WriteStartElement(elementName);

            if (attributes != null)
            {
                foreach (var pair in attributes)
                {
                    writer.WriteAttributeString(pair.Key, pair.Value);
                }
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write a string.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        private static void WriteString(XmlWriter writer, string value, SerializationContext context)
        {
            int index;

            // Write a string
            if (value == string.Empty || context.AmfVersion != AmfVersion.Amf3
                || (index = context.StringReferences.IndexOf(value)) == -1)
            {
                if (value != string.Empty)
                {
                    context.StringReferences.Add(value);
                }

                WriteElement(writer, AmfxContent.String, value);
            }
                
                // Write a string reference. Only in AMF+
            else
            {
                var attributes = new Dictionary<string, string> { { AmfxContent.StringId, index.ToString() } };
                WriteEmptyElement(writer, AmfxContent.String, attributes);
            }
        }

        /// <summary>
        /// Write an XML.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        private static void WriteXml(XmlWriter writer, XmlDocument value, SerializationContext context)
        {
            int index;

            // Write an XML
            if (context.AmfVersion != AmfVersion.Amf3 || (index = context.References.IndexOf(value)) == -1)
            {
                context.References.Add(new AmfReference { Reference = value, AmfxType = AmfxContent.Xml });

                string content;

                using (var stream = new MemoryStream())
                {
                    value.Save(stream);
                    content = Encoding.UTF8.GetString(stream.ToArray());
                }

                WriteElement(writer, AmfxContent.Xml, content);
            }
                
                // Write an XML reference. Only in AMF+
            else
            {
                var attributes = new Dictionary<string, string> { { AmfxContent.ReferenceId, index.ToString() } };
                WriteEmptyElement(writer, AmfxContent.Reference, attributes);
            }
        }

        #endregion

        /// <summary>
        ///     Data contract descriptor.
        /// </summary>
        private class DataContractDescriptor
        {
            #region Public Properties

            /// <summary>
            ///     Data contract type's alias.
            /// </summary>
            public string Alias { get; set; }

            /// <summary>
            ///     Data contract type's AMFX type.
            /// </summary>
            public string AmfxType { get; set; }

            /// <summary>
            ///     Data contract field map.
            /// </summary>
            public IEnumerable<KeyValuePair<string, FieldInfo>> FieldMap { get; set; }

            /// <summary>
            ///     Type is a primitive type.
            /// </summary>
            public bool IsPrimitive { get; set; }

            /// <summary>
            ///     Data contract property map.
            /// </summary>
            public IEnumerable<KeyValuePair<string, PropertyInfo>> PropertyMap { get; set; }

            /// <summary>
            ///     Data contract type type.
            /// </summary>
            public Type Type { get; set; }

            #endregion
        }

        /// <summary>
        ///     Enum descriptor.
        /// </summary>
        private class EnumDescriptor : DataContractDescriptor
        {
            #region Public Properties

            /// <summary>
            ///     Enumeration values.
            /// </summary>
            public Dictionary<object, object> Values { get; set; }

            #endregion
        }

        /// <summary>
        ///     AMFX serialization context.
        /// </summary>
        private sealed class SerializationContext : AmfContext
        {
            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="SerializationContext"/> class.
            /// </summary>
            /// <param name="version">
            /// The version.
            /// </param>
            public SerializationContext(AmfVersion version)
                : base(version)
            {
            }

            #endregion

            #region Public Properties

            /// <summary>
            ///     Contains the types that may be present in the object graph.
            /// </summary>
            public Dictionary<string, DataContractDescriptor> KnownTypes { get; set; }

            #endregion

            #region Public Methods and Operators

            /// <summary>
            /// Get type's descriptor.
            /// </summary>
            /// <param name="type">
            /// The type.
            /// </param>
            /// <exception cref="SerializationException">
            /// Type is not registered.
            /// </exception>
            /// <returns>
            /// The <see cref="DataContractDescriptor"/>.
            /// </returns>
            public DataContractDescriptor GetDescriptor(Type type)
            {
                return (from pair in this.KnownTypes where pair.Value.Type == type select pair.Value).FirstOrDefault();
            }

            /// <summary>
            /// Get enum type's descriptor.
            /// </summary>
            /// <param name="type">
            /// The type.
            /// </param>
            /// <exception cref="SerializationException">
            /// Type is not registered.
            /// </exception>
            /// <returns>
            /// The <see cref="EnumDescriptor"/>.
            /// </returns>
            public EnumDescriptor GetEnumDescriptor(Type type)
            {
                return this.GetDescriptor(type) as EnumDescriptor;
            }

            #endregion
        }
    }
}