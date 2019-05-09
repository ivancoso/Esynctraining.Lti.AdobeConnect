//using Esynctraining.Lti.Lms.Common.API;
//using Esynctraining.Lti.Lms.Common.API.AgilixBuzz;
//using Esynctraining.Lti.Lms.Common.Constants;
//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using System.Xml.Linq;
//using System.Xml.XPath;

//namespace Esynctraining.Lti.Lms.AgilixBuzz
//{
//    /// <summary>
//    ///     The course API.
//    /// </summary>
//    // ReSharper disable once InconsistentNaming
//    public sealed partial class DlapAPI : ILmsAPI, IAgilixBuzzApi
//    {

//        internal async Task<(long? result, string error)> GetLastSignalIdAsync(Dictionary<string, object> licenseSettings, BuzzApiClient session = null)
//        {
//            long? result = null;
//            var (signalsResult, error) = await this.LoginIfNecessaryAsync(session, s => s.GetAsync(Commands.Signals.GetLastSignalId), licenseSettings);
//            if (signalsResult == null)
//            {
//                error = error ?? "DLAP. Unable to retrive result from API";
//                return (null, error);
//            }

//            if (!BuzzApiClient.IsSuccess(signalsResult))
//            {
//                error = "DLAP. Unable to create user: " + BuzzApiClient.GetMessage(signalsResult);
//                _logger.Error(error);
//            }

//            try
//            {
//                var signal = signalsResult.XPathSelectElement("/signal");
//                result = long.Parse(signal.XPathEvaluate("string(@id)").ToString());
//            }
//            catch (Exception ex)
//            {
//                error = ex.ToString();
//            }

//            return (result, error);
//        }

//        internal async Task<(Course course, string error)> GetCourseAsync(
//            Dictionary<string, object> licenseSettings,
//            int courseId,
//            BuzzApiClient session = null)
//        {
//            var (courseResult, error) = await this.LoginIfNecessaryAsync(
//                session,
//                s => s.GetAsync(Commands.Courses.GetOne, string.Format(Parameters.Courses.GetOne, courseId).ToDictionary()),
//                licenseSettings);

//            if (courseResult == null)
//            {
//                error = error ?? "DLAP. Unable to retrive course from API";
//                return (null, error);
//            }

//            if (!BuzzApiClient.IsSuccess(courseResult))
//            {
//                error = "DLAP. Unable to get course: " + BuzzApiClient.GetMessage(courseResult);
//                _logger.Error(error);
//            }

//            if (session != null)
//            {
//                var course = courseResult.XPathSelectElement("/course");
//                if (course != null)
//                {
//                    var courseName = course.XPathEvaluate("string(@title)").ToString();
//                    var courseStartDate = course.XPathEvaluate("string(@startdate)").ToString();
//                    var courseEndDate = course.XPathEvaluate("string(@enddate)").ToString();

//                    return (new Course
//                    {
//                        CourseId = courseId,
//                        Title = courseName,
//                        StartDate = courseStartDate,
//                        EndDate = courseEndDate,
//                    }, error);
//                }

//                error = "Course not found";
//            }

//            return (null, error);
//        }


//        internal async Task<(List<Signal> result, string error)> GetSignalsListAsync(
//            Dictionary<string, object> licenseSettings,
//            BuzzApiClient session = null,
//            string types = "1.2|4.3|4.8")
//        {
//            var result = new List<Signal>();
//            var (signalsResult, error) = await this.LoginIfNecessaryAsync(
//                session,
//                s => s.GetAsync(Commands.Signals.List, string.Format(Parameters.Signals.List, licenseSettings[LmsLicenseSettingNames.AgilixBuzzLastSignalId], s.DomainId, types).ToDictionary()),
//                licenseSettings);

//            if (signalsResult == null)
//            {
//                error = error ?? "DLAP. Unable to retrive result from API";
//                return (result, error);
//            }

//            if (!BuzzApiClient.IsSuccess(signalsResult))
//            {
//                error = "DLAP. Unable to create user: " + BuzzApiClient.GetMessage(signalsResult);
//                _logger.Error(error);
//            }

//            IEnumerable<XElement> signals = signalsResult.XPathSelectElements("/signals/signal");
//            foreach (XElement signal in signals)
//            {
//                var signalId = long.Parse(signal.XPathEvaluate("string(@signalid)").ToString());
//                var entityid = int.Parse(signal.XPathEvaluate("string(@entityid)").ToString());
//                var type = signal.XPathEvaluate("string(@type)").ToString();
//                var data = signal.XPathSelectElement("data");
//                if (data != null)
//                {
//                    switch (type)
//                    {
//                        case SignalTypes.EnrollmentChanged:
//                            ProcessEnrollment(signalId, entityid, type, data, signal, result);
//                            break;
//                        case SignalTypes.CourseDeleted:
//                            ProcessCourseSignal(data, result, signalId, entityid, type);

//                            break;
//                        case SignalTypes.CourseCreated:
//                            ProcessCourseSignal(data, result, signalId, entityid, type);

//                            break;
//                    }
//                }
//            }

//            return (result, error);
//        }

//        /// <summary>
//        /// The process enrollment.
//        /// </summary>
//        /// <param name="signalId">
//        /// The signal id.
//        /// </param>
//        /// <param name="entityid">
//        /// The entity id.
//        /// </param>
//        /// <param name="type">
//        /// The type.
//        /// </param>
//        /// <param name="data">
//        /// The data.
//        /// </param>
//        /// <param name="signal">
//        /// The signal.
//        /// </param>
//        /// <param name="result">
//        /// The result.
//        /// </param>
//        private static void ProcessEnrollment(long signalId, int entityid, string type, XElement data, XElement signal, List<Signal> result)
//        {
//            var enrollmentSignal = new EnrollmentSignal(signalId, entityid, type)
//            {
//                OldStatus = int.Parse(data.XPathEvaluate("string(@oldstatus)").ToString()),
//                NewStatus = int.Parse(data.XPathEvaluate("string(@newstatus)").ToString())
//            };
//            if (enrollmentSignal.NewStatus != 0)
//            {
//                enrollmentSignal.OldRole = ProcessRole(signal.XPathEvaluate("string(@oldflags)").ToString());
//                enrollmentSignal.NewRole = ProcessRole(signal.XPathEvaluate("string(@newflags)").ToString());
//            }

//            result.Add(enrollmentSignal);
//        }

//        private static void ProcessCourseSignal(XElement data, List<Signal> result, long signalId, int entityid, string type)
//        {
//            var changedEntityType = data.XPathEvaluate("string(@entitytype)").ToString();
//            if (changedEntityType == "C")
//            {
//                var itemId = data.XPathEvaluate("string(@itemid)").ToString();
//                result.Add(new CourseSignal(signalId, entityid, type) { EntityType = changedEntityType, ItemId = itemId });
//            }
//        }

//        /// <summary>
//        /// The signal types.
//        /// </summary>
//        public class SignalTypes
//        {
//            /// <summary>
//            /// The enrollment changed.
//            /// </summary>
//            public const string EnrollmentChanged = "1.2";

//            /// <summary>
//            /// The course deleted.
//            /// </summary>
//            public const string CourseDeleted = "4.3";

//            /// <summary>
//            /// The course created.
//            /// </summary>
//            public const string CourseCreated = "4.8";
//        }
//    }
//}
