namespace PDFAnnotation.Core.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    /// <summary>
    /// The points converter.
    /// </summary>
    public static class PointsConverter
    {
        #region Public Methods and Operators

        /// <summary>
        /// The parse.
        /// </summary>
        /// <param name="points">
        /// The points.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{T}"/>.
        /// </returns>
        public static IEnumerable<Point> Parse(string points)
        {
            string[] array = points.Split(new[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string pointStr in array)
            {
                string[] pointCoordinates = pointStr.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                float x, y;
                if (pointCoordinates.Length == 2
                    && float.TryParse(pointCoordinates[0], NumberStyles.Any, CultureInfo.InvariantCulture, out x)
                    && float.TryParse(pointCoordinates[1], NumberStyles.Any, CultureInfo.InvariantCulture, out y))
                {
                    yield return new Point { X = x, Y = y };
                }
            }
        }

        #endregion
    }
}