namespace PDFAnnotation.Core.Domain.DTO.Enums
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// The border styles. <see href="http://www.verypdf.com/document/pdf-format-reference/pg_0612.htm" />.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1201:ElementsMustAppearInTheCorrectOrder",
        Justification = "Reviewed. Suppression is OK here.")]
    public enum StickyNoteIcons
    {
        /// <summary>
        /// The comment.
        /// </summary>
        Comment = 1,

        /// <summary>
        /// The help.
        /// </summary>
        Help = 2,

        /// <summary>
        /// The insert.
        /// </summary>
        Insert = 3,

        /// <summary>
        /// The key.
        /// </summary>
        Key = 4,

        /// <summary>
        /// The new paragraph.
        /// </summary>
        NewParagraph = 5,

        /// <summary>
        /// The note.
        /// </summary>
        Note = 6,

        /// <summary>
        /// The paraghraph.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        Paraghraph = 7,

        /// <summary>
        /// The checkmark.
        /// </summary>
        Checkmark = 8,

        /// <summary>
        /// The circle.
        /// </summary>
        Circle = 9,

        /// <summary>
        /// The cross.
        /// </summary>
        Cross = 10,

        /// <summary>
        /// The right arrow.
        /// </summary>
        RightArrow = 11,

        /// <summary>
        /// The right pointer.
        /// </summary>
        RightPointer = 12,

        /// <summary>
        /// The star.
        /// </summary>
        Star = 13,

        /// <summary>
        /// The up arrow.
        /// </summary>
        UpArrow = 14,

        /// <summary>
        /// The up left arrow.
        /// </summary>
        UpLeftArrow = 15
    }

    /// <summary>
    ///     The border style names.
    /// </summary>
    public class StickyNoteIconsNames
    {
        #region Static Fields

        /// <summary>
        /// The checkmark.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate",
            Justification = "Reviewed. Suppression is OK here.")]
        public static readonly string Checkmark = "Checkmark";

        /// <summary>
        /// The circle.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate",
            Justification = "Reviewed. Suppression is OK here.")]
        public static readonly string Circle = "Circle";

        /// <summary>
        /// The comment.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate",
            Justification = "Reviewed. Suppression is OK here.")]
        public static readonly string Comment = "Comment";

        /// <summary>
        /// The cross.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate",
            Justification = "Reviewed. Suppression is OK here.")]
        public static readonly string Cross = "Cross";

        /// <summary>
        /// The help.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate",
            Justification = "Reviewed. Suppression is OK here.")]
        public static readonly string Help = "Help";

        /// <summary>
        /// The insert.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate",
            Justification = "Reviewed. Suppression is OK here.")]
        public static readonly string Insert = "Insert";

        /// <summary>
        /// The key.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate",
            Justification = "Reviewed. Suppression is OK here.")]
        public static readonly string Key = "Key";

        /// <summary>
        /// The new paragraph.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate",
            Justification = "Reviewed. Suppression is OK here.")]
        public static readonly string NewParagraph = "NewParagraph";

        /// <summary>
        /// The note.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate",
            Justification = "Reviewed. Suppression is OK here.")]
        public static readonly string Note = "Note";

        /// <summary>
        /// The paraghraph.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here."), SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate",
            Justification = "Reviewed. Suppression is OK here.")]
        public static readonly string Paraghraph = "Paraghraph";

        /// <summary>
        /// The right arrow.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate",
            Justification = "Reviewed. Suppression is OK here.")]
        public static readonly string RightArrow = "RightArrow";

        /// <summary>
        /// The right pointer.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate",
            Justification = "Reviewed. Suppression is OK here.")]
        public static readonly string RightPointer = "RightPointer";

        /// <summary>
        /// The star.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate",
            Justification = "Reviewed. Suppression is OK here.")]
        public static readonly string Star = "Star";

        /// <summary>
        /// The up arrow.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate",
            Justification = "Reviewed. Suppression is OK here.")]
        public static readonly string UpArrow = "UpArrow";

        /// <summary>
        /// The up left arrow.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate",
            Justification = "Reviewed. Suppression is OK here.")]
        public static readonly string UpLeftArrow = "UpLeftArrow";

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The get name.
        /// </summary>
        /// <param name="input">
        /// The input.
        /// </param>
        /// <returns>
        /// The <see cref="PdfName"/>.
        /// </returns>
        public static string GetName(string input)
        {
            StickyNoteIcons enumResult;
            if (Enum.TryParse(input, out enumResult))
            {
                switch (enumResult)
                {
                    case StickyNoteIcons.Comment:
                        return Comment;
                    case StickyNoteIcons.Help:
                        return Help;
                    case StickyNoteIcons.Insert:
                        return Insert;
                    case StickyNoteIcons.Key:
                        return Key;
                    case StickyNoteIcons.NewParagraph:
                        return NewParagraph;
                    case StickyNoteIcons.Note:
                        return Note;
                    case StickyNoteIcons.Paraghraph:
                        return Paraghraph;
                    case StickyNoteIcons.Checkmark:
                        return Checkmark;
                    case StickyNoteIcons.Circle:
                        return Circle;
                    case StickyNoteIcons.Cross:
                        return Cross;
                    case StickyNoteIcons.RightArrow:
                        return RightArrow;
                    case StickyNoteIcons.RightPointer:
                        return RightPointer;
                    case StickyNoteIcons.Star:
                        return Star;
                    case StickyNoteIcons.UpArrow:
                        return UpArrow;
                    case StickyNoteIcons.UpLeftArrow:
                        return UpLeftArrow;
                }
            }
            else
            {
                foreach (var name in Enum.GetNames(typeof(StickyNoteIcons)))
                {
                    if (string.Equals(name, input, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return GetName(name);
                    }
                }
            }

            return Note;
        }

        #endregion

    }

}
