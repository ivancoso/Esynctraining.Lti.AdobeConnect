namespace Esynctraining.Core.Domain.Entities
{
    using System;
    using System.Globalization;

    /// <summary>
    ///     The date time with zone.
    /// </summary>
    public class DateTimeWithZone
    {
        #region Constants

        /// <summary>
        ///     The date format.
        /// </summary>
        public const string DateFormat = @"yyyy-MM-dd\THH:mm:ss";

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimeWithZone"/> class.
        /// </summary>
        /// <param name="dateTime">
        /// The date time.
        /// </param>
        /// <param name="timeZone">
        /// The time zone.
        /// </param>
        public DateTimeWithZone(DateTime dateTime, TimeZoneInfo timeZone)
        {
            this.DateTime = dateTime;
            this.TimeZone = timeZone;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the date.
        /// </summary>
        public DateTimeWithZone Date
        {
            get
            {
                return new DateTimeWithZone(this.DateTime.Date, this.TimeZone);
            }
        }

        /// <summary>
        /// Gets a value indicating whether is daylight saving time.
        /// </summary>
        public bool IsDaylightSavingTime
        {
            get
            {
                return this.TimeZone.IsDaylightSavingTime(this.DateTime);
            }
        }

        /// <summary>
        ///     Gets or sets the date time.
        /// </summary>
        public DateTime DateTime { get; set; }

        /// <summary>
        ///     Gets or sets the time zone.
        /// </summary>
        public TimeZoneInfo TimeZone { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The add hours.
        /// </summary>
        /// <param name="hours">
        /// The hours.
        /// </param>
        /// <returns>
        /// The <see cref="DateTimeWithZone"/>.
        /// </returns>
        public DateTimeWithZone AddHours(int hours)
        {
            return new DateTimeWithZone(this.DateTime.AddHours(hours), this.TimeZone);
        }

        /// <summary>
        /// The to target time zone.
        /// </summary>
        /// <param name="targetTimeZone">
        /// The target Time Zone.
        /// </param>
        /// <returns>
        /// The <see cref="DateTimeWithZone"/>.
        /// </returns>
        public DateTimeWithZone To(TimeZoneInfo targetTimeZone)
        {
            return new DateTimeWithZone(TimeZoneInfo.ConvertTime(this.DateTime, this.TimeZone, targetTimeZone), targetTimeZone);
        }

        /// <summary>
        /// The to EST.
        /// </summary>
        /// <returns>
        /// The <see cref="DateTimeWithZone"/>.
        /// </returns>
        public DateTimeWithZone ToEst()
        {
            TimeZoneInfo targetTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            return this.To(targetTimeZone);
        }

        /// <summary>
        ///     The to string.
        /// </summary>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public override string ToString()
        {
            TimeSpan offset = this.TimeZone.GetUtcOffset(this.DateTime);
            return this.DateTime.ToString(DateFormat) + string.Format("{0}:{1}", offset.Hours.ToString("D2"), offset.Minutes.ToString("D2"));
        }

        /// <summary>
        ///     The to string.
        /// </summary>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public string ToString(string format)
        {
            return this.DateTime.ToString(format);
        }

        /// <summary>
        ///     The to string.
        /// </summary>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            return this.DateTime.ToString(format, formatProvider);
        }

        /// <summary>
        ///     The to string.
        /// </summary>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public string ToSmallString()
        {
            return this.DateTime.ToString(DateFormat);
        }

        /// <summary>
        /// The to invariant culture string.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string ToInvariantCultureString()
        {
            return this.DateTime.ToString(CultureInfo.InvariantCulture);
        }

        #endregion
    }
}