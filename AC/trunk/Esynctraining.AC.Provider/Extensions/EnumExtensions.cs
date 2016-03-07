// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumExtensions.cs" company="eSyncTraining">
//   eSyncTraining
// </copyright>
// <summary>
//   Enumeration extensions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Esynctraining.AC.Provider.Extensions
{
    using System;

    /// <summary>
    /// Enumeration extensions.
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// The to string.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string ToXmlString(this Enum value)
        {
            try
            {
                var name = Enum.GetName(value.GetType(), value);

                if (name != null)
                {
                    return name.Replace('_', '-').ToLower();
                }
            }
            catch (Exception ex)
            {
                TraceTool.TraceException(ex);
            }

            return null;
        }

    }

}
