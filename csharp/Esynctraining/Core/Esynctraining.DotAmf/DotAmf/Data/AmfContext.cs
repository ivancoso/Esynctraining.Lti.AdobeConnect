// --------------------------------------------------------------------------------------------------------------------
// <copyright company="" file="AmfContext.cs">
//   
// </copyright>
// <summary>
//   AMF context.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace DotAmf.Data
{
    using System.Collections.Generic;

    /// <summary>
    ///     AMF context.
    /// </summary>
    internal class AmfContext
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AmfContext"/> class.
        /// </summary>
        /// <param name="version">
        /// The version.
        /// </param>
        public AmfContext(AmfVersion version)
        {
            this.AmfVersion = version;

            this.References = new List<AmfReference>();
            this.StringReferences = new List<string>();
            this.TraitsReferences = new List<AmfTypeTraits>();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     AMF version.
        /// </summary>
        public AmfVersion AmfVersion { get; private set; }

        /// <summary>
        ///     Object references.
        /// </summary>
        public List<AmfReference> References { get; private set; }

        /// <summary>
        ///     String references.
        /// </summary>
        public List<string> StringReferences { get; private set; }

        /// <summary>
        ///     Traits references.
        /// </summary>
        public List<AmfTypeTraits> TraitsReferences { get; private set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Reset reference counter.
        /// </summary>
        public void ResetReferences()
        {
            this.References.Clear();
            this.StringReferences.Clear();
            this.TraitsReferences.Clear();
        }

        /// <summary>
        /// Get a traits index.
        /// </summary>
        /// <param name="alias">
        /// The alias.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int TraitsIndex(string alias)
        {
            for (int i = 0; i < this.TraitsReferences.Count; i++)
            {
                if (this.TraitsReferences[i].TypeName == alias)
                {
                    return i;
                }
            }

            return -1;
        }

        #endregion
    }

    /// <summary>
    ///     AMF reference.
    /// </summary>
    internal struct AmfReference
    {
        #region Fields

        /// <summary>
        ///     AMFX type name.
        /// </summary>
        public string AmfxType;

        /// <summary>
        ///     Object reference.
        /// </summary>
        public object Reference;

        #endregion
    }

    #region Extension

    /// <summary>
    ///     AMF encoding context extension.
    /// </summary>
    internal static class AmfEncodingContextExtension
    {
        #region Public Methods and Operators

        /// <summary>
        /// Get reference index.
        /// </summary>
        /// <param name="list">
        /// The list.
        /// </param>
        /// <param name="reference">
        /// The reference.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public static int IndexOf(this List<AmfReference> list, object reference)
        {
            if (reference == null)
            {
                return -1;
            }

            for (int i = 0; i < list.Count; i++)
            {
                AmfReference proxy = list[i];

                if (proxy.Reference == null)
                {
                    continue;
                }

                if (list[i].Reference == reference)
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Track a reference.
        /// </summary>
        /// <param name="list">
        /// The list.
        /// </param>
        public static void Track(this IList<AmfReference> list)
        {
            list.Add(default(AmfReference));
        }

        #endregion
    }

    #endregion
}