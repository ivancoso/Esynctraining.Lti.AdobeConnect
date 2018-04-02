namespace PDFAnnotation.Core.Constants
{
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// The display formats.
    /// </summary>
    public static class EntityTypes
    {
        #region Constants

        /// <summary>
        /// The confidential.
        /// </summary>
        public const string Confidential = "confidential";

        /// <summary>
        /// The confidential attorney.
        /// </summary>
        public const string ConfidentialAttorney = "confidentialattorney";

        /// <summary>
        /// The highly confidential.
        /// </summary>
        public const string HighlyConfidential = "highlyconfidential";

        /// <summary>
        /// The exhibit.
        /// </summary>
        public const string Exhibit = "exhibit";

        /// <summary>
        /// The exhibit.
        /// </summary>
        public const string ExhibitPlaintiff = "exhibitplaintiff";

        /// <summary>
        /// The exhibit.
        /// </summary>
        public const string ExhibitDefendant = "exhibitdefendant";

        /// <summary>
        /// The drawing.
        /// </summary>
        public const string Drawing = "drawing";

        /// <summary>
        /// The highlight.
        /// </summary>
        public const string Highlight = "highlight";

        /// <summary>
        /// The strikeout.
        /// </summary>
        public const string StrikeOut = "strikeout";

        /// <summary>
        /// The rectangle.
        /// </summary>
        public const string Rectangle = "rectangle";

        /// <summary>
        /// The ellipse.
        /// </summary>
        public const string Ellipse = "ellipse";

        /// <summary>
        /// The rectangle.
        /// </summary>
        public const string FilledRectangle = "filledRectangle";

        /// <summary>
        /// The ellipse.
        /// </summary>
        public const string FilledEllipse = "filledEllipse";

        /// <summary>
        /// The line.
        /// </summary>
        public const string Line = "line";

        /// <summary>
        /// The arrow to left.
        /// </summary>
        public const string ArrowLeft = "arrowleft";

        /// <summary>
        /// The arrow to both.
        /// </summary>
        public const string ArrowBoth = "arrowboth";

        /// <summary>
        /// The arrow to both.
        /// </summary>
        public const string Arrow = "arrow";

        /// <summary>
        /// The arrow to right.
        /// </summary>
        public const string ArrowRight = "arrowright";

        /// <summary>
        /// The right mark.
        /// </summary>
        public const string Right = "right";

        /// <summary>
        /// The wrong mark.
        /// </summary>
        public const string Wrong = "wrong";

        /// <summary>
        /// The text.
        /// </summary>
        public const string Text = "text";

        /// <summary>
        /// The text.
        /// </summary>
        public const string Rotation = "rotation";

        /// <summary>
        /// The picture.
        /// </summary>
        public const string Picture = "picture";


        /// <summary>
        /// The formula.
        /// </summary>
        public const string Formula = "formula";


        /// <summary>
        /// The formula.
        /// </summary>
        public const string Annotation = "stickyNote";

        /// <summary>
        /// The stamp.
        /// </summary>
        public const string Stamp = "stamp";

        /// <summary>
        /// Page rules (grid in WTS)
        /// </summary>
        public const string PageRules = "pageRules";


        /// <summary>
        /// The highlight line
        /// </summary>
        public const string HighlightLine = "highlightLine";

        #endregion

        static EntityTypes()
        {
            var type = typeof(EntityTypes);
            var fieldInfos =
                type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy).ToList();

            //// Go through the list and only pick out the constants
            //// IsLiteral determines if its value is written at 
            ////   compile time and not changeable
            //// IsInitOnly determine if the field can be set 
            ////   in the body of the constructor
            //// for C# a field which is readonly keyword would have both true 
            ////   but a const field would have only IsLiteral equal to true
            var constants = fieldInfos.Where(fi => fi.IsLiteral && !fi.IsInitOnly).ToList();
            var types = constants.Select(fi => fi.GetRawConstantValue().ToString()).ToList();
            SupportedTypes = new ReadOnlyCollection<string>(types);
        }

        /// <summary>
        /// Gets all the constants from a particular
        /// type including the constants from all the base types
        /// </summary>
        public static ReadOnlyCollection<string> SupportedTypes { get; private set; }

    }

}