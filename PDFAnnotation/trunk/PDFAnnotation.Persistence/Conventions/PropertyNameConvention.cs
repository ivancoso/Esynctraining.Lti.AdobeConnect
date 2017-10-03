namespace PDFAnnotation.Persistence.Conventions
{
    using System.Collections.Generic;
    /// <summary>
    /// The property name convention.
    /// </summary>
    public class PropertyNameConvention 
    {
        #region Fields

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

        #endregion
    }
}