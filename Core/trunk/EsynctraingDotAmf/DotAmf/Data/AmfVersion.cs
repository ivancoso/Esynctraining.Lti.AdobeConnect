// --------------------------------------------------------------------------------------------------------------------
// <copyright company="" file="AmfVersion.cs">
//   
// </copyright>
// <summary>
//   AMF packet version.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace DotAmf.Data
{
    using System;

    /// <summary>
    ///     AMF packet version.
    /// </summary>
    public enum AmfVersion : ushort
    {
        /// <summary>
        ///     AMF0.
        /// </summary>
        Amf0 = 0, 

        /// <summary>
        ///     AMF3.
        /// </summary>
        Amf3 = 3
    }

    #region Extension

    /// <summary>
    ///     The amf version extension.
    /// </summary>
    internal static class AmfVersionExtension
    {
        #region Public Methods and Operators

        /// <summary>
        /// Convert version enumeration value to an AMFX value.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string ToAmfxName(this AmfVersion value)
        {
            switch (value)
            {
                case AmfVersion.Amf0:
                    return AmfxContent.VersionAmf0;

                case AmfVersion.Amf3:
                    return AmfxContent.VersionAmf3;

                default:
                    throw new NotSupportedException("Version '" + value + "' is not supported.");
            }
        }

        #endregion
    }

    #endregion
}