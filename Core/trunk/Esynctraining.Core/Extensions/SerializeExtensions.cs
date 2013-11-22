namespace Esynctraining.Core.Extensions
{
    using System;
    using System.IO;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;

    /// <summary>
    /// Represents extensions for XML serialization and deserialization.
    /// </summary>
    public static class SerializeExtension
    {
        /// <summary>
        /// Empty array of types.
        /// </summary>
        public static readonly Type[] EmptyTypesArray = new Type[] { };

        /// <summary>
        /// Default serializer encoding.
        /// </summary>
        public static readonly Encoding DefaultEncoding = Encoding.Unicode;

        /// <summary>
        /// Serialize object.
        /// </summary>
        /// <typeparam name="T">Object type.</typeparam>
        /// <param name="self">Extended object.</param>
        /// <param name="encoding">Character encoding.</param>
        /// <param name="extraTypes">Types used during serialization.</param>
        /// <returns>Serialized string.</returns>
        public static string Serialize<T>(this T self, Encoding encoding, Type[] extraTypes)
        {
            using (var memoryStream = new MemoryStream())
            {
                var xmlSerializer = new XmlSerializer(self.GetType(), extraTypes);
                using (var xmlText = new XmlTextWriter(memoryStream, encoding) { Formatting = Formatting.Indented })
                {
                    var namespaces = new XmlSerializerNamespaces();
                    namespaces.Add(string.Empty, string.Empty);

                    xmlSerializer.Serialize(xmlText, self, namespaces);
                    return encoding.GetString(memoryStream.ToArray());
                }
            }
        }

        /// <summary>
        /// Deserialize object.
        /// </summary>
        /// <typeparam name="T">Object type.</typeparam>
        /// <param name="self">Extended object.</param>
        /// <param name="encoding">Character encoding.</param>
        /// <param name="extraTypes">Types used during deserialization.</param>
        /// <returns>Deserialized object.</returns>
        public static T Deserialize<T>(this string self, Encoding encoding, Type[] extraTypes)
        {
            var buffer = encoding.GetBytes(self);
            using (var memoryStream = new MemoryStream(buffer))
            {
                var serializer = new XmlSerializer(typeof(T), extraTypes);
                return (T)serializer.Deserialize(memoryStream);
            }
        }

        /// <summary>
        /// Serialize object.
        /// </summary>
        /// <typeparam name="T">Object type.</typeparam>
        /// <param name="self">Extended object.</param>
        /// <param name="extraTypes">Types used during serialization.</param>
        /// <returns>Serialized string.</returns>
        public static string Serialize<T>(this T self, Type[] extraTypes)
        {
            return self.Serialize(DefaultEncoding, extraTypes);
        }

        /// <summary>
        /// Deserialize object.
        /// </summary>
        /// <typeparam name="T">Object type.</typeparam>
        /// <param name="self">Extended object.</param>
        /// <param name="extraTypes">Types used during deserialization.</param>
        /// <returns>Deserialized object.</returns>
        public static T Deserialize<T>(this string self, Type[] extraTypes)
        {
            return self.Deserialize<T>(DefaultEncoding, extraTypes);
        }

        /// <summary>
        /// Serialize object.
        /// </summary>
        /// <typeparam name="T">Object type.</typeparam>
        /// <param name="self">Extended object.</param>
        /// <returns>Serialized string.</returns>
        public static string Serialize<T>(this T self)
        {
            return self.Serialize(EmptyTypesArray);
        }

        /// <summary>
        /// Deserialize object.
        /// </summary>
        /// <typeparam name="T">Object type.</typeparam>
        /// <param name="self">Extended object.</param>
        /// <returns>Deserialized object.</returns>
        public static T Deserialize<T>(this string self)
        {
            return self.Deserialize<T>(DefaultEncoding, EmptyTypesArray);
        }
    }
}
