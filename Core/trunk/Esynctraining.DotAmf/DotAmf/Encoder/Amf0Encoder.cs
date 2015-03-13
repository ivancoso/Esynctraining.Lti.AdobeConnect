// --------------------------------------------------------------------------------------------------------------------
// <copyright company="" file="Amf0Encoder.cs">
//   
// </copyright>
// <summary>
//   AMF0 encoder.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace DotAmf.Encoder
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Xml;

    using DotAmf.Data;
    using DotAmf.IO;

    /// <summary>
    ///     AMF0 encoder.
    /// </summary>
    internal class Amf0Encoder : AbstractAmfEncoder
    {
        #region Constants

        /// <summary>
        ///     Maximum number of byte a short string can contain.
        /// </summary>
        private const uint ShortStringLimit = 65535;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Amf0Encoder"/> class.
        /// </summary>
        /// <param name="encodingOptions">
        /// The encoding options.
        /// </param>
        public Amf0Encoder(AmfEncodingOptions encodingOptions)
            : base(encodingOptions)
        {
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The encode.
        /// </summary>
        /// <param name="stream">
        /// The stream.
        /// </param>
        /// <param name="input">
        /// The input.
        /// </param>
        public override void Encode(Stream stream, XmlReader input)
        {
            var writer = new AmfStreamWriter(stream);
            AmfContext context = this.CreateDefaultContext();
            this.WriteAmfValue(context, input, writer);
        }

        /// <summary>
        /// The write packet body.
        /// </summary>
        /// <param name="stream">
        /// The stream.
        /// </param>
        /// <param name="descriptor">
        /// The descriptor.
        /// </param>
        public override void WritePacketBody(Stream stream, AmfMessageDescriptor descriptor)
        {
            var writer = new AmfStreamWriter(stream);

            WriteUtf8(writer, descriptor.Target);
            WriteUtf8(writer, descriptor.Response);
            writer.Write(-1);
        }

        /// <summary>
        /// The write packet header.
        /// </summary>
        /// <param name="stream">
        /// The stream.
        /// </param>
        /// <param name="descriptor">
        /// The descriptor.
        /// </param>
        public override void WritePacketHeader(Stream stream, AmfHeaderDescriptor descriptor)
        {
            var writer = new AmfStreamWriter(stream);

            WriteUtf8(writer, descriptor.Name);
            writer.Write((byte)(descriptor.MustUnderstand ? 1 : 0));
            writer.Write(-1);
        }

        #endregion

        #region Methods

        /// <summary>
        /// The write amf value.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="input">
        /// The input.
        /// </param>
        /// <param name="writer">
        /// The writer.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        /// <exception cref="XmlException">
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// </exception>
        protected override void WriteAmfValue(AmfContext context, XmlReader input, AmfStreamWriter writer)
        {
            if (context.AmfVersion != AmfVersion.Amf0)
            {
                throw new InvalidOperationException(
                    string.Format(Errors.Amf0Decoder_ReadAmfValue_AmfVersionNotSupported, context.AmfVersion));
            }

            if (input == null)
            {
                throw new ArgumentNullException("input");
            }

            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (input.NodeType != XmlNodeType.Element)
            {
                throw new XmlException(string.Format("Element node expected, {0} found.", input.NodeType));
            }

            switch (input.Name)
            {
                case AmfxContent.Null:
                    WriteNull(writer);
                    return;

                case AmfxContent.True:
                    WriteBoolean(writer, true);
                    return;

                case AmfxContent.False:
                    WriteBoolean(writer, false);
                    return;
            }

            

            XmlReader reader = input.ReadSubtree();
            reader.MoveToContent();

            switch (reader.Name)
            {
                case AmfxContent.Integer:
                case AmfxContent.Double:
                    WriteNumber(writer, reader);
                    break;

                case AmfxContent.String:
                    WriteString(writer, reader);
                    break;

                case AmfxContent.Date:
                    WriteDate(writer, reader);
                    break;

                case AmfxContent.Xml:
                    WriteXml(writer, reader);
                    break;

                case AmfxContent.Reference:
                    WriteReference(context, writer, reader);
                    break;

                case AmfxContent.Array:
                    this.WriteArray(context, writer, reader);
                    break;

                case AmfxContent.Object:
                    this.WriteObject(context, writer, reader);
                    break;

                default:
                    throw new NotSupportedException("Unexpected AMFX type: " + reader.Name);
            }

            reader.Close();

            
        }

        /// <summary>
        /// Write a boolean value.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        private static void WriteBoolean(AmfStreamWriter writer, bool value)
        {
            writer.Write(value);
        }

        /// <summary>
        /// Write a date.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        /// <param name="input">
        /// The input.
        /// </param>
        private static void WriteDate(AmfStreamWriter writer, XmlReader input)
        {
            double value = Convert.ToDouble(input.ReadString());
            WriteDate(writer, value);
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
        private static void WriteDate(AmfStreamWriter writer, double value)
        {
            WriteTypeMarker(writer, Amf0TypeMarker.Date);
            writer.Write(value);
            writer.Write((short)0); // Timezone (not used)
        }

        /// <summary>
        /// Write a <c>null</c>.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        private static void WriteNull(AmfStreamWriter writer)
        {
            WriteTypeMarker(writer, Amf0TypeMarker.Null);
        }

        /// <summary>
        /// Write a number.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        /// <param name="input">
        /// The input.
        /// </param>
        private static void WriteNumber(AmfStreamWriter writer, XmlReader input)
        {
            WriteTypeMarker(writer, Amf0TypeMarker.Number);

            double value = Convert.ToDouble(input.ReadString());
            writer.Write(value);
        }

        /// <summary>
        /// Write a reference value.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="writer">
        /// The writer.
        /// </param>
        /// <param name="input">
        /// The input.
        /// </param>
        private static void WriteReference(AmfContext context, AmfStreamWriter writer, XmlReader input)
        {
            int index = Convert.ToInt32(input.GetAttribute(AmfxContent.ReferenceId));
            AmfReference proxy = context.References[index];

            switch (proxy.AmfxType)
            {
                case AmfxContent.Array:
                case AmfxContent.Object:
                    WriteTypeMarker(writer, Amf0TypeMarker.Reference);
                    writer.Write((ushort)index);
                    break;

                default:
                    throw new InvalidOperationException(
                        string.Format("AMFX type '{0}' cannot be send by reference.", proxy.AmfxType));
            }
        }

        /// <summary>
        /// Write a string.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        /// <param name="input">
        /// The input.
        /// </param>
        private static void WriteString(AmfStreamWriter writer, XmlReader input)
        {
            string value = input.IsEmptyElement ? string.Empty : input.ReadString();

            WriteString(writer, value);
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
        private static void WriteString(AmfStreamWriter writer, string value)
        {
            byte[] data = Encoding.UTF8.GetBytes(value);

            WriteTypeMarker(writer, data.Length < ShortStringLimit ? Amf0TypeMarker.String : Amf0TypeMarker.LongString);

            WriteUtf8(writer, data);
        }

        /// <summary>
        /// Write an AMF0 type marker.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        /// <param name="marker">
        /// The marker.
        /// </param>
        private static void WriteTypeMarker(AmfStreamWriter writer, Amf0TypeMarker marker)
        {
            writer.Write((byte)marker);
        }

        /// <summary>
        /// Write UTF8 string.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        private static void WriteUtf8(AmfStreamWriter writer, string value)
        {
            byte[] data = Encoding.UTF8.GetBytes(value);
            WriteUtf8(writer, data);
        }

        /// <summary>
        /// Write UTF8 data.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        /// <param name="data">
        /// The data.
        /// </param>
        private static void WriteUtf8(AmfStreamWriter writer, byte[] data)
        {
            if (data.Length < ShortStringLimit)
            {
                writer.Write((ushort)data.Length);
            }
            else
            {
                writer.Write((uint)data.Length);
            }

            writer.Write(data);
        }

        /// <summary>
        /// Write an XML.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        /// <param name="input">
        /// The input.
        /// </param>
        private static void WriteXml(AmfStreamWriter writer, XmlReader input)
        {
            string value = input.ReadString();
            byte[] data = Encoding.UTF8.GetBytes(value);
            WriteXml(writer, data);
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
        private static void WriteXml(AmfStreamWriter writer, byte[] value)
        {
            WriteTypeMarker(writer, Amf0TypeMarker.XmlDocument);
            writer.Write((uint)value.Length);
            writer.Write(value);
        }

        /// <summary>
        /// Write an array.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="writer">
        /// The writer.
        /// </param>
        /// <param name="input">
        /// The input.
        /// </param>
        private void WriteArray(AmfContext context, AmfStreamWriter writer, XmlReader input)
        {
            context.References.Add(new AmfReference { AmfxType = AmfxContent.Array });

            uint length = Convert.ToUInt32(input.GetAttribute(AmfxContent.ArrayLength));
            writer.Write(length);

            if (length == 0)
            {
                return;
            }

            input.MoveToContent();

            while (input.Read())
            {
                if (input.NodeType != XmlNodeType.Element)
                {
                    continue;
                }

                for (int i = 0; i < length; i++)
                {
                    XmlReader itemreader = input.ReadSubtree();
                    itemreader.MoveToContent();
                    this.WriteAmfValue(context, itemreader, writer);
                    itemreader.Close();
                }
            }
        }

        /// <summary>
        /// Write an object.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="writer">
        /// The writer.
        /// </param>
        /// <param name="input">
        /// The input.
        /// </param>
        private void WriteObject(AmfContext context, AmfStreamWriter writer, XmlReader input)
        {
            context.References.Add(new AmfReference { AmfxType = AmfxContent.Object });

            WriteTypeMarker(writer, Amf0TypeMarker.Object);

            string typeName = string.Empty;

            if (input.HasAttributes)
            {
                typeName = input.GetAttribute(AmfxContent.ObjectType);
            }

            var traits = new AmfTypeTraits { TypeName = typeName };

            while (input.Read())
            {
                if (input.NodeType != XmlNodeType.Element && input.Name != AmfxContent.Traits)
                {
                    continue;
                }

                if (!input.IsEmptyElement)
                {
                    XmlReader traitsReader = input.ReadSubtree();
                    traitsReader.MoveToContent();
                    traitsReader.ReadStartElement();

                    var members = new List<string>();

                    while (input.NodeType != XmlNodeType.EndElement)
                    {
                        members.Add(traitsReader.ReadElementContentAsString());
                    }

                    traits.ClassMembers = members.ToArray();
                    traitsReader.Close();
                }

                break;
            }

            

            // Untyped object
            if (string.IsNullOrEmpty(traits.TypeName))
            {
                WriteTypeMarker(writer, Amf0TypeMarker.Object);
            }
                
                // Strongly-typed object
            else
            {
                WriteTypeMarker(writer, Amf0TypeMarker.TypedObject);
                byte[] typeNameData = Encoding.UTF8.GetBytes(traits.TypeName);
                writer.Write((ushort)typeNameData.Length);
                writer.Write(typeNameData);
            }

            

            #region Write members

            int i = 0;

            while (input.Read())
            {
                if (input.NodeType != XmlNodeType.Element)
                {
                    continue;
                }

                string memberName = traits.ClassMembers[i];
                XmlReader memberReader = input.ReadSubtree();
                memberReader.MoveToContent();

                WriteUtf8(writer, memberName);
                this.WriteAmfValue(context, memberReader, writer);

                memberReader.Close();
                i++;
            }

            #endregion

            WriteUtf8(writer, string.Empty);
            WriteTypeMarker(writer, Amf0TypeMarker.ObjectEnd);
        }

        #endregion
    }
}