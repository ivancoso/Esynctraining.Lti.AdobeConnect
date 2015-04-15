namespace EdugameCloud.Lti.Domain.Entities
{
    /// <summary>
    ///     The schedule type.
    /// </summary>
    public enum ScheduleType : byte
    {
        /// <summary>
        ///     The daily.
        /// </summary>
        Daily = 0, 

        /// <summary>
        ///     The weekly.
        /// </summary>
        Weekly = 1, 

        /// <summary>
        ///     The monthly.
        /// </summary>
        Monthly = 2, 

        /// <summary>
        ///     The hourly.
        /// </summary>
        Hourly = 3, 

        /// <summary>
        /// The every single minute.
        /// </summary>
        EverySingleMinute = 4, 

        /// <summary>
        /// The every 5 minutes.
        /// </summary>
        Every5Minutes = 5, 

        /// <summary>
        /// The every 10 minutes.
        /// </summary>
        Every10Minutes = 6, 

        /// <summary>
        /// The every 15 minutes.
        /// </summary>
        Every15Minutes = 7, 

        /// <summary>
        /// The every 20 minutes.
        /// </summary>
        Every20Minutes = 8, 

        /// <summary>
        /// The every 25 minutes.
        /// </summary>
        Every25Minutes = 9, 

        /// <summary>
        /// The every half an hour.
        /// </summary>
        EveryHalfAnHour = 10, 

        /// <summary>
        /// The every 45 minutes.
        /// </summary>
        Every45Minutes = 11,

        /// <summary>
        /// The custom
        /// </summary>
        Custom = 12, 
    }
}