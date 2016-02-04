//using System;
//namespace EdugameCloud.Lti.Extensions
//{
//    public static class DateConverter
//    {
//        private static readonly DateTime _origin;
//        private static readonly DateTime _originLocalTime;
        

//        static DateConverter()
//        {
//            _origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
//            _originLocalTime = _origin.ToLocalTime();
//        }


//        /// <summary>
//        /// Convert a <c>DateTime</c> to a UNIX timestamp in milliseconds.
//        /// </summary>
//        /// <param name="value">
//        /// The value.
//        /// </param>
//        /// <returns>
//        /// The <see cref="double"/>.
//        /// </returns>
//        public static double ConvertToUnixTimestamp(this DateTime value)
//        {
//            if (value.Kind != DateTimeKind.Utc)
//            {
//                return (value - _originLocalTime).TotalSeconds * 1000;
//            }
//            return (value - _origin).TotalSeconds * 1000;
//        }

//    }

//}
