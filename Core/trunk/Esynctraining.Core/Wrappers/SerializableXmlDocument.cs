namespace Esynctraining.Core.Wrappers
{
    using System;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Xml;

    /// <summary>
    /// The serializable xml document.
    /// </summary>
    [Serializable]
    public class SerializableXmlDocument : ISerializable
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableXmlDocument"/> class.
        /// </summary>
        public SerializableXmlDocument()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableXmlDocument"/> class.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        public SerializableXmlDocument(XmlDocument value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableXmlDocument"/> class.
        /// </summary>
        /// <param name="info">
        /// The info.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        public SerializableXmlDocument(SerializationInfo info, StreamingContext context)
        {
            var serializedData = (byte[])info.GetValue("doc", typeof(byte[]));
            if (null != serializedData)
            {
                this.Value = Deserialize(serializedData);
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public XmlDocument Value { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The op_ implicit.
        /// </summary>
        /// <param name="doc">
        /// The doc.
        /// </param>
        /// <returns>
        /// </returns>
        public static implicit operator SerializableXmlDocument(XmlDocument doc)
        {
            return new SerializableXmlDocument(doc);
        }

        /// <summary>
        /// The op_ implicit.
        /// </summary>
        /// <param name="sdoc">
        /// The serializable document.
        /// </param>
        /// <returns>
        /// </returns>
        public static implicit operator XmlDocument(SerializableXmlDocument sdoc)
        {
            return sdoc.Value;
        }

        /// <summary>
        /// The get object data.
        /// </summary>
        /// <param name="info">
        /// The info.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            byte[] serializedData = null;
            if (null != this.Value)
            {
                serializedData = Serialize(this.Value);
            }

            info.AddValue("doc", serializedData);
        }

        #endregion

        #region Methods

        /// <summary>
        /// The deserialize.
        /// </summary>
        /// <param name="serializedData">
        /// The serialized data.
        /// </param>
        /// <returns>
        /// The <see cref="XmlDocument"/>.
        /// </returns>
        private static XmlDocument Deserialize(byte[] serializedData)
        {
            var doc = new XmlDocument();
            doc.Load(new MemoryStream(serializedData, false));
            return doc;
        }

        /// <summary>
        /// The serialize.
        /// </summary>
        /// <param name="doc">
        /// The document.
        /// </param>
        /// <returns>
        /// The byte array/>.
        /// </returns>
        private static byte[] Serialize(XmlDocument doc)
        {
            var stream = new MemoryStream();
            doc.Save(stream);
            return stream.ToArray();
        }

        #endregion
    }
}