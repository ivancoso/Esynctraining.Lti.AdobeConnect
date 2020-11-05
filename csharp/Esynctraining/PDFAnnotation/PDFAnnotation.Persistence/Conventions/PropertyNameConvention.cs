namespace PDFAnnotation.Persistence.Conventions
{
    using System.Collections.Generic;

    /// <summary>
    /// The property name convention.
    /// </summary>
    public class PropertyNameConvention 
    {

        /// <summary>
        ///     The exceptions.
        /// </summary>
        public static readonly List<string> Exceptions = new List<string>
        {
            "StateName",
            "FileName",
            "CategoryName",
            "CompanyName",
        };

    }

}