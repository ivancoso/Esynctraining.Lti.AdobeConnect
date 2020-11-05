namespace PDFAnnotation.Core.Wrappers
{
    using System;
    using System.Globalization;
    using System.Text.RegularExpressions;

    using iTextSharp.text.pdf;

    /// <summary>
    /// The ccpdf date.
    /// </summary>
    public class CustomPDFDate : PdfDate
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomPDFDate"/> class.
        /// </summary>
        /// <param name="d">
        /// The d.
        /// </param>
        public CustomPDFDate(DateTime d)
            : base(d)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomPDFDate"/> class.
        /// </summary>
        public CustomPDFDate()
        {
        }

        public CustomPDFDate(string eventDate, string timeStamp)
        {
            Match r =
                new Regex(
                    @"(?<year>(\d){4})-(?<month>(\d){2})-(?<day>(\d){2})T(?<hour>(\d){2}):(?<min>(\d){2}):(?<sec>(\d){2})(?<tzh>(\+|\-)(\d){2})?:?(?<tzm>(\d){2})?")
                    .Match(eventDate);
            Match t = new Regex(@"(?<hour>(\d){2}):(?<min>(\d){2}):(?<sec>(\d){2})").Match(timeStamp);
            if (r.Success)
            {
                var d = new DateTime(
                    GetInt(r, "year"),
                    GetInt(r, "month"),
                    GetInt(r, "day"),
                    t.Success ? GetInt(t, "hour") : GetInt(r, "hour"),
                    t.Success ? GetInt(t, "min") : GetInt(r, "min"),
                    t.Success ? GetInt(t, "sec") : GetInt(r, "sec"),
                    DateTimeKind.Unspecified);


                this.value = d.ToString("\\D\\:yyyyMMddHHmmss", DateTimeFormatInfo.InvariantInfo);
                if (HasTimeZoneGroup(r, "tzh"))
                {
                    this.value = this.value + r.Groups["tzh"].Value + "'"
                                 + (HasTimeZoneGroup(r, "tzm") ? r.Groups["tzm"].Value : "00") + "'";
                }
                else
                {
                    this.value = this.value + "-00'00'";
                }
            }
            else
            {
                this.value = new CustomPDFDate(DateTime.Now).Value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CCPDFDate"/> class.
        /// </summary>
        /// <param name="w3cDTF">
        /// The w 3 c dtf.
        /// </param>
        public CustomPDFDate(string w3cDTF)
        {
            Match r =
                new Regex(
                    @"(?<year>(\d){4})-(?<month>(\d){2})-(?<day>(\d){2})T(?<hour>(\d){2}):(?<min>(\d){2}):(?<sec>(\d){2})(?<tzh>(\+|\-)(\d){2})?:?(?<tzm>(\d){2})?")
                    .Match(w3cDTF);
            if (r.Success)
            {
                var d = new DateTime(
                    GetInt(r, "year"),
                    GetInt(r, "month"),
                    GetInt(r, "day"),
                    GetInt(r, "hour"),
                    GetInt(r, "min"),
                    GetInt(r, "sec"),
                    DateTimeKind.Unspecified);
                this.value = d.ToString("\\D\\:yyyyMMddHHmmss", DateTimeFormatInfo.InvariantInfo);
                if (HasTimeZoneGroup(r, "tzh"))
                {
                    this.value = this.value + r.Groups["tzh"].Value + "'"
                                 + (HasTimeZoneGroup(r, "tzm") ? r.Groups["tzm"].Value : "00") + "'";
                }
                else
                {
                    this.value = this.value + "-00'00'";
                }
            }
            else
            {
                this.value = new CustomPDFDate(DateTime.Now).Value;
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the value.
        /// </summary>
        public string Value
        {
            get
            {
                return this.value;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets integer from group.
        /// </summary>
        /// <param name="match">
        /// The match.
        /// </param>
        /// <param name="groupName">
        /// The group name.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        private static int GetInt(Match match, string groupName)
        {
            try
            {
                if (match.Groups[groupName] != null)
                {
                    return ToInt(match.Groups[groupName].Value);
                }
            }
            catch
            {
            }

            return default(int);
        }

        /// <summary>
        /// Gets integer from group.
        /// </summary>
        /// <param name="match">
        /// The match.
        /// </param>
        /// <param name="tzGroupName">
        /// The tz Group Name.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        private static bool HasTimeZoneGroup(Match match, string tzGroupName)
        {
            try
            {
                return match.Groups[tzGroupName] != null;
            }
            catch
            {
            }

            return false;
        }

        /// <summary>
        /// Converts string to integer.
        /// </summary>
        /// <param name="val">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        private static int ToInt(string val)
        {
            int res;
            if (int.TryParse(val, out res))
            {
                return res;
            }

            return default(int);
        }

        #endregion
    }
}
