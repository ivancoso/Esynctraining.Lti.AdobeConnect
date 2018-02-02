namespace EdugameCloud.Lti.AgilixBuzz
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using System.Xml.XPath;
    using API;
    using EdugameCloud.Lti.API.AgilixBuzz;
    using EdugameCloud.Lti.Domain.Entities;
    using Esynctraining.Core.Extensions;

    /// <summary>
    ///     The course API.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public sealed partial class DlapAPI : ILmsAPI, IAgilixBuzzApi
    {
        //#region Public Methods and Operators

        ///// <summary>
        ///// The login and create a session.
        ///// </summary>
        ///// <param name="error">
        ///// The error.
        ///// </param>
        ///// <param name="lmsDomain">
        ///// The LMS domain.
        ///// </param>
        ///// <param name="userName">
        ///// The user name.
        ///// </param>
        ///// <param name="password">
        ///// The password.
        ///// </param>
        ///// <returns>
        ///// The <see cref="Session"/>.
        ///// </returns>
        //internal Session LoginAndCreateASession(out string error, string lmsDomain, string userName, string password)
        //{
        //    try
        //    {
        //        var session = new Session("EduGameCloud", (string)this.settings.AgilixBuzzApiUrl) { Verbose = true };
        //        string userPrefix = lmsDomain.ToLower()
        //            .Replace(".agilixbuzz.com", string.Empty)
        //            .Replace("www.", string.Empty);

        //        XElement result = session.Login(userPrefix, userName, password);
        //        if (!Session.IsSuccess(result))
        //        {
        //            error = "DLAP. Unable to login: " + Session.GetMessage(result);
        //            this.logger.Error(error);
        //            return null;
        //        }

        //        error = null;
        //        session.DomainId = result.XPathEvaluate("string(user/@domainid)").ToString();
        //        return session;
        //    }
        //    catch (Exception ex)
        //    {
        //        error = ex.Message;
        //        return null;
        //    }
        //}

        //public bool LoginAndCheckSession(out string error, string lmsDomain, string userName, string password)
        //{
        //    Session session = LoginAndCreateASession(out error, lmsDomain, userName, password);
        //    return session != null;
        //}

        /////// <summary>
        /////// The create announcement.
        /////// </summary>
        /////// <param name="credentials">
        /////// The credentials.
        /////// </param>
        /////// <param name="courseId">
        /////// The course id.
        /////// </param>
        /////// <param name="announcementTitle">
        /////// The announcement title.
        /////// </param>
        /////// <param name="announcementMessage">
        /////// The announcement message.
        /////// </param>
        /////// <param name="error">
        /////// The error.
        /////// </param>
        /////// <param name="session">
        /////// The session.
        /////// </param>
        ////public void CreateAnnouncement(LmsCompany credentials, int courseId, string announcementTitle, string announcementMessage, out string error, Session session = null)
        ////{
        ////    XElement courseResult = this.LoginIfNecessary(
        ////        session,
        ////        s =>
        ////        s.Get(Commands.Courses.GetOne, string.Format(Parameters.Courses.GetOne, courseId).ToParams()),
        ////        credentials,
        ////        out error);
        ////    if (courseResult == null)
        ////    {
        ////        error = error ?? "DLAP. Unable to retrive course from API";
        ////        return;
        ////    }

        ////    if (!Session.IsSuccess(courseResult))
        ////    {
        ////        error = "DLAP. Unable to get course: " + Session.GetMessage(courseResult);
        ////        this.logger.Error(error);
        ////    }

        ////    if (session != null)
        ////    {
        ////        /*[<roles>
        ////        When entityid refers to a course or section, the recipient roles within the course or section for this announcement.
        ////        <role flags="enum" />
        ////        . . .
        ////        </roles>]*/
        ////        var course = courseResult.XPathSelectElement("/course");
        ////        var courseName = course.XPathEvaluate("string(@title)").ToString();
        ////        var courseStartDate = course.XPathEvaluate("string(@startdate)").ToString();
        ////        var courseEndDate = course.XPathEvaluate("string(@enddate)").ToString();
        ////        var announcementName = Guid.NewGuid().ToString("N") + ".zip";
        ////        //// var groupsXml = this.FormatGroupsXml(session, courseId);

        ////        // ReSharper disable once UnusedVariable
        ////        XElement announcementResult = session.Post(
        ////            Commands.Announcements.Put + "&"
        ////            + string.Format(Parameters.Announcements.Put, courseId, announcementName),
        ////            new XElement(
        ////                "announcement",
        ////                new XAttribute("to", courseName),
        ////                new XAttribute("entityid", courseId),
        ////                new XAttribute("title", announcementTitle),
        ////                new XAttribute("startdate", courseStartDate),
        ////                new XAttribute("enddate", courseEndDate),
        ////                new XAttribute("recurse", "false"),
        ////                new XElement("body", new XCData(announcementMessage))));
        ////    }
        ////}

        ///// <summary>
        ///// The get users for course.
        ///// </summary>
        ///// <param name="company">
        ///// The company.
        ///// </param>
        ///// <param name="courseid">
        ///// The course id.
        ///// </param>
        ///// <param name="error">
        ///// The error.
        ///// </param>
        ///// <param name="session">
        ///// The session.
        ///// </param>
        ///// <returns>
        ///// The <see cref="List{LmsUserDTO}"/>.
        ///// </returns>
        //public List<LmsUserDTO> GetUsersForCourse(
        //    LmsCompany company, 
        //    int courseid, 
        //    out string error, 
        //    object extraData)
        //{
        //    Session session = extraData as Session;

        //    var result = new List<LmsUserDTO>();
        //    XElement enrollmentsResult = this.LoginIfNecessary(
        //        session, 
        //        s =>
        //        s.Get(
        //            Commands.Enrollments.List, 
        //            string.Format(Parameters.Enrollments.List, s.DomainId, courseid).ToParams()), 
        //            company,
        //            out error);
        //    if (enrollmentsResult == null)
        //    {
        //        error = error ?? "DLAP. Unable to retrive result from API";
        //        return result;
        //    }

        //    if (!Session.IsSuccess(enrollmentsResult))
        //    {
        //        error = "DLAP. Unable to create user: " + Session.GetMessage(enrollmentsResult);
        //        this.logger.Error(error);
        //    }

        //    IEnumerable<XElement> enrollments = enrollmentsResult.XPathSelectElements("/enrollments/enrollment");
        //    foreach (XElement enrollment in enrollments)
        //    {
        //        string privileges = enrollment.XPathEvaluate("string(@privileges)").ToString();
        //        string status = enrollment.XPathEvaluate("string(@status)").ToString();
        //        XElement user = enrollment.XPathSelectElement("user");
        //        if (!string.IsNullOrWhiteSpace(privileges) && user != null && this.IsEnrollmentActive(status))
        //        {
        //            var role = ProcessRole(privileges);
        //            string userId = user.XPathEvaluate("string(@id)").ToString();
        //            string firstName = user.XPathEvaluate("string(@firstname)").ToString();
        //            string lastName = user.XPathEvaluate("string(@lastname)").ToString();
        //            string userName = user.XPathEvaluate("string(@username)").ToString();
        //            string email = user.XPathEvaluate("string(@email)").ToString();
        //            result.Add(
        //                new LmsUserDTO
        //                    {
        //                        lms_role = role, 
        //                        primary_email = email, 
        //                        login_id = userName, 
        //                        id = userId, 
        //                        name = firstName + " " + lastName, 
        //                    });
        //        }
        //    }

        //    return result;
        //}

        ///// <summary>
        ///// The is enrollment active.
        ///// </summary>
        ///// <param name="enrollmentStatus">
        ///// The enrollment status.
        ///// </param>
        ///// <returns>
        ///// The <see cref="bool"/>.
        ///// </returns>
        //public bool IsEnrollmentActive(string enrollmentStatus)
        //{
        //    switch (enrollmentStatus)
        //    {
        //        case "4": ////Withdrawn
        //        case "5": ////WithdrawnFailed
        //        case "6": ////Transfered
        //        case "9": ////Suspended
        //        case "10": ////Inactive
        //            return false;
        //    }

        //    return true;
        //}

        ///// <summary>
        ///// The get users for course.
        ///// </summary>
        ///// <param name="company">
        ///// The company.
        ///// </param>
        ///// <param name="courseId">
        ///// The course Id.
        ///// </param>
        ///// <param name="error">
        ///// The error.
        ///// </param>
        ///// <param name="session">
        ///// The session.
        ///// </param>
        ///// <returns>
        ///// The <see cref="Course"/>.
        ///// </returns>
        //internal Course GetCourse(
        //    LmsCompany company,
        //    int courseId,
        //    out string error,
        //    Session session = null)
        //{
        //    XElement courseResult = this.LoginIfNecessary(
        //        session,
        //        s =>
        //        s.Get(Commands.Courses.GetOne, string.Format(Parameters.Courses.GetOne, courseId).ToParams()),
        //        company,
        //        out error);
        //    if (courseResult == null)
        //    {
        //        error = error ?? "DLAP. Unable to retrive course from API";
        //        return null;
        //    }

        //    if (!Session.IsSuccess(courseResult))
        //    {
        //        error = "DLAP. Unable to get course: " + Session.GetMessage(courseResult);
        //        this.logger.Error(error);
        //    }

        //    if (session != null)
        //    {
        //        var course = courseResult.XPathSelectElement("/course");
        //        if (course != null)
        //        {
        //            var courseName = course.XPathEvaluate("string(@title)").ToString();
        //            var courseStartDate = course.XPathEvaluate("string(@startdate)").ToString();
        //            var courseEndDate = course.XPathEvaluate("string(@enddate)").ToString();

        //            return new Course
        //                       {
        //                           CourseId = courseId,
        //                           Title = courseName,
        //                           StartDate = courseStartDate,
        //                           EndDate = courseEndDate,
        //                       };
        //        }

        //        error = "Course not found";
        //    }

        //    return null;
        //}

        ///// <summary>
        ///// The get enrollment.
        ///// </summary>
        ///// <param name="company">
        ///// The company.
        ///// </param>
        ///// <param name="enrollmentId">
        ///// The enrollment Id.
        ///// </param>
        ///// <param name="error">
        ///// The error.
        ///// </param>
        ///// <param name="session">
        ///// The session.
        ///// </param>
        ///// <returns>
        ///// The <see cref="Enrollment"/>.
        ///// </returns>
        //internal Enrollment GetEnrollment(
        //    LmsCompany company,
        //    long enrollmentId,
        //    out string error,
        //    Session session = null)
        //{
        //    XElement enrollmentResult = this.LoginIfNecessary(
        //        session,
        //        s =>
        //        s.Get(Commands.Enrollments.GetOne, string.Format(Parameters.Enrollments.GetOne, enrollmentId).ToParams()),
        //        company,
        //        out error);
        //    if (enrollmentResult == null)
        //    {
        //        error = error ?? "DLAP. Unable to retrive enrollment from API";
        //        return null;
        //    }

        //    if (!Session.IsSuccess(enrollmentResult))
        //    {
        //        error = "DLAP. Unable to get course: " + Session.GetMessage(enrollmentResult);
        //        this.logger.Error(error);
        //    }

        //    if (session != null)
        //    {
        //        var enrollment = enrollmentResult.XPathSelectElement("/enrollment");
        //        if (enrollment != null)
        //        {
        //            var userId = enrollment.XPathEvaluate("string(@userid)").ToString();
        //            var courseId = int.Parse(enrollment.XPathEvaluate("string(@courseid)").ToString());
        //            var role = enrollment.XPathEvaluate("string(@privileges)").ToString();
        //            var status = enrollment.XPathEvaluate("string(@status)").ToString();
        //            var user = enrollment.XPathSelectElement("user");
        //            var email = user.XPathEvaluate("string(@email)").ToString();
        //            var userName = user.XPathEvaluate("string(@username)").ToString();
        //            return new Enrollment
        //            {
        //                CourseId = courseId,
        //                UserId = userId,
        //                Role = ProcessRole(role),
        //                Email = email,
        //                UserName = userName,
        //                Status = status,
        //            };
        //        }

        //        error = "Enrollment not found";
        //    }

        //    return null;
        //}

        ///// <summary>
        ///// The get last signal id.
        ///// </summary>
        ///// <param name="company">
        ///// The company.
        ///// </param>
        ///// <param name="error">
        ///// The error.
        ///// </param>
        ///// <param name="session">
        ///// The session.
        ///// </param>
        ///// <returns>
        ///// The <see cref="Nullable{Int64}"/>.
        ///// </returns>
        //internal long? GetLastSignalId(LmsCompany company, out string error, Session session = null)
        //{
        //    long? result = null;
        //    XElement signalsResult = this.LoginIfNecessary(session, s => s.Get(Commands.Signals.GetLastSignalId), company, out error);
        //    if (signalsResult == null)
        //    {
        //        error = error ?? "DLAP. Unable to retrive result from API";
        //        return null;
        //    }

        //    if (!Session.IsSuccess(signalsResult))
        //    {
        //        error = "DLAP. Unable to create user: " + Session.GetMessage(signalsResult);
        //        this.logger.Error(error);
        //    }

        //    try
        //    {
        //        var signal = signalsResult.XPathSelectElement("/signal");
        //        result = long.Parse(signal.XPathEvaluate("string(@id)").With(x => x.ToString()));
        //    }
        //    catch (Exception ex)
        //    {
        //        error = ex.ToString();
        //    }

        //    return result;
        //}

        /// <summary>
        /// The get last signal id.
        /// </summary>
        /// <param name="company">
        /// The company.
        /// </param>
        /// <param name="error">
        /// The error.
        /// </param>
        /// <param name="session">
        /// The session.
        /// </param>
        /// <returns>
        /// The <see cref="Nullable{Int64}"/>.
        /// </returns>
        internal async Task<(long? result, string error)> GetLastSignalIdAsync(LmsCompany company, Session session = null)
        {
            long? result = null;
            var (signalsResult, error) = await this.LoginIfNecessaryAsync(session, s => s.GetAsync(Commands.Signals.GetLastSignalId), company);
            if (signalsResult == null)
            {
                error = error ?? "DLAP. Unable to retrive result from API";
                return (null, error);
            }

            if (!Session.IsSuccess(signalsResult))
            {
                error = "DLAP. Unable to create user: " + Session.GetMessage(signalsResult);
                _logger.Error(error);
            }

            try
            {
                var signal = signalsResult.XPathSelectElement("/signal");
                result = long.Parse(signal.XPathEvaluate("string(@id)").With(x => x.ToString()));
            }
            catch (Exception ex)
            {
                error = ex.ToString();
            }

            return (result, error);
        }

        internal async Task<(Course course, string error)> GetCourseAsync(
            LmsCompany company,
            int courseId,
            Session session = null)
        {
            var (courseResult, error) = await this.LoginIfNecessaryAsync(
                session,
                s => s.GetAsync(Commands.Courses.GetOne, string.Format(Parameters.Courses.GetOne, courseId).ToDictionary()),
                company);

            if (courseResult == null)
            {
                error = error ?? "DLAP. Unable to retrive course from API";
                return (null, error);
            }

            if (!Session.IsSuccess(courseResult))
            {
                error = "DLAP. Unable to get course: " + Session.GetMessage(courseResult);
                _logger.Error(error);
            }

            if (session != null)
            {
                var course = courseResult.XPathSelectElement("/course");
                if (course != null)
                {
                    var courseName = course.XPathEvaluate("string(@title)").ToString();
                    var courseStartDate = course.XPathEvaluate("string(@startdate)").ToString();
                    var courseEndDate = course.XPathEvaluate("string(@enddate)").ToString();

                    return (new Course
                    {
                        CourseId = courseId,
                        Title = courseName,
                        StartDate = courseStartDate,
                        EndDate = courseEndDate,
                    }, error);
                }

                error = "Course not found";
            }

            return (null, error);
        }

        /// <summary>
        /// The get enrollment.
        /// </summary>
        /// <param name="company">
        /// The company.
        /// </param>
        /// <param name="enrollmentId">
        /// The enrollment Id.
        /// </param>
        /// <param name="error">
        /// The error.
        /// </param>
        /// <param name="session">
        /// The session.
        /// </param>
        /// <returns>
        /// The <see cref="Enrollment"/>.
        /// </returns>
        internal async Task<(Enrollment result, string error)> GetEnrollmentAsync(
            LmsCompany company,
            long enrollmentId,
            Session session = null)
        {
            var (enrollmentResult, error) = await this.LoginIfNecessaryAsync(
                session,
                s => s.GetAsync(Commands.Enrollments.GetOne, string.Format(Parameters.Enrollments.GetOne, enrollmentId).ToDictionary()),
                company);

            if (enrollmentResult == null)
            {
                error = error ?? "DLAP. Unable to retrive enrollment from API";
                return (null, error);
            }

            if (!Session.IsSuccess(enrollmentResult))
            {
                error = "DLAP. Unable to get course: " + Session.GetMessage(enrollmentResult);
                _logger.Error(error);
            }

            if (session != null)
            {
                var enrollment = enrollmentResult.XPathSelectElement("/enrollment");
                if (enrollment != null)
                {
                    var userId = enrollment.XPathEvaluate("string(@userid)").ToString();
                    var courseId = int.Parse(enrollment.XPathEvaluate("string(@courseid)").ToString());
                    var role = enrollment.XPathEvaluate("string(@privileges)").ToString();
                    var status = enrollment.XPathEvaluate("string(@status)").ToString();
                    var user = enrollment.XPathSelectElement("user");
                    var email = user.XPathEvaluate("string(@email)").ToString();
                    var userName = user.XPathEvaluate("string(@username)").ToString();

                    return (new Enrollment
                    {
                        CourseId = courseId,
                        UserId = userId,
                        Role = ProcessRole(role),
                        Email = email,
                        UserName = userName,
                        Status = status,
                    }, error);
                }

                error = "Enrollment not found";
            }

            return (null, error);
        }

        internal async Task<(List<Signal> result, string error)> GetSignalsListAsync(
            LmsCompany company,
            Session session = null,
            string types = "1.2|4.3|4.8")
        {
            var result = new List<Signal>();
            var (signalsResult, error) = await this.LoginIfNecessaryAsync(
                session,
                s => s.GetAsync(Commands.Signals.List, string.Format(Parameters.Signals.List, company.LastSignalId, s.DomainId, types).ToDictionary()),
                company);

            if (signalsResult == null)
            {
                error = error ?? "DLAP. Unable to retrive result from API";
                return (result, error);
            }

            if (!Session.IsSuccess(signalsResult))
            {
                error = "DLAP. Unable to create user: " + Session.GetMessage(signalsResult);
                _logger.Error(error);
            }

            IEnumerable<XElement> signals = signalsResult.XPathSelectElements("/signals/signal");
            foreach (XElement signal in signals)
            {
                var signalId = long.Parse(signal.XPathEvaluate("string(@signalid)").With(x => x.ToString()));
                var entityid = int.Parse(signal.XPathEvaluate("string(@entityid)").With(x => x.ToString()));
                var type = signal.XPathEvaluate("string(@type)").ToString();
                var data = signal.XPathSelectElement("data");
                if (data != null)
                {
                    switch (type)
                    {
                        case SignalTypes.EnrollmentChanged:
                            ProcessEnrollment(signalId, entityid, type, data, signal, result);
                            break;
                        case SignalTypes.CourseDeleted:
                            ProcessCourseSignal(data, result, signalId, entityid, type);

                            break;
                        case SignalTypes.CourseCreated:
                            ProcessCourseSignal(data, result, signalId, entityid, type);

                            break;
                    }
                }
            }

            return (result, error);
        }

        /// <summary>
        /// The process enrollment.
        /// </summary>
        /// <param name="signalId">
        /// The signal id.
        /// </param>
        /// <param name="entityid">
        /// The entity id.
        /// </param>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <param name="signal">
        /// The signal.
        /// </param>
        /// <param name="result">
        /// The result.
        /// </param>
        private static void ProcessEnrollment(long signalId, int entityid, string type, XElement data, XElement signal, List<Signal> result)
        {
            var enrollmentSignal = new EnrollmentSignal(signalId, entityid, type)
            {
                OldStatus = int.Parse(data.XPathEvaluate("string(@oldstatus)").With(x => x.ToString())),
                NewStatus = int.Parse(data.XPathEvaluate("string(@newstatus)").With(x => x.ToString()))
            };
            if (enrollmentSignal.NewStatus != 0)
            {
                enrollmentSignal.OldRole = ProcessRole(signal.XPathEvaluate("string(@oldflags)").With(x => x.ToString()));
                enrollmentSignal.NewRole = ProcessRole(signal.XPathEvaluate("string(@newflags)").With(x => x.ToString()));
            }

            result.Add(enrollmentSignal);
        }
       
        //#endregion

        //#region Methods

        ///// <summary>
        ///// The process enrollment.
        ///// </summary>
        ///// <param name="signalId">
        ///// The signal id.
        ///// </param>
        ///// <param name="entityid">
        ///// The entity id.
        ///// </param>
        ///// <param name="type">
        ///// The type.
        ///// </param>
        ///// <param name="data">
        ///// The data.
        ///// </param>
        ///// <param name="signal">
        ///// The signal.
        ///// </param>
        ///// <param name="result">
        ///// The result.
        ///// </param>
        //private static void ProcessEnrollment(long signalId, int entityid, string type, XElement data, XElement signal, List<Signal> result)
        //{
        //    var enrollmentSignal = new EnrollmentSignal(signalId, entityid, type)
        //                               {
        //                                   OldStatus = int.Parse(data.XPathEvaluate("string(@oldstatus)").With(x => x.ToString())),
        //                                   NewStatus = int.Parse(data.XPathEvaluate("string(@newstatus)").With(x => x.ToString()))
        //                               };
        //    if (enrollmentSignal.NewStatus != 0)
        //    {
        //        enrollmentSignal.OldRole = ProcessRole(signal.XPathEvaluate("string(@oldflags)").With(x => x.ToString()));
        //        enrollmentSignal.NewRole = ProcessRole(signal.XPathEvaluate("string(@newflags)").With(x => x.ToString()));
        //    }

        //    result.Add(enrollmentSignal);
        //}

        private static void ProcessCourseSignal(XElement data, List<Signal> result, long signalId, int entityid, string type)
        {
            var changedEntityType = data.XPathEvaluate("string(@entitytype)").With(x => x.ToString());
            if (changedEntityType == "C")
            {
                var itemId = data.XPathEvaluate("string(@itemid)").With(x => x.ToString());
                result.Add(new CourseSignal(signalId, entityid, type) { EntityType = changedEntityType, ItemId = itemId });
            }
        }

        ///// <summary>
        ///// The process role.
        ///// </summary>
        ///// <param name="privileges">
        ///// The privileges.
        ///// </param>
        ///// <returns>
        ///// The <see cref="string"/>.
        ///// </returns>
        //private static string ProcessRole(string privileges)
        //{
        //    string role = Roles.Student;
        //    long privilegesVal;
        //    if (long.TryParse(privileges, out privilegesVal))
        //    {
        //        if (CheckRole(privilegesVal, RightsFlags.ControlCourse))
        //        {
        //            role = Roles.Owner;
        //        }
        //        else if (CheckRole(privilegesVal, RightsFlags.ReadCourse)
        //                 && CheckRole(privilegesVal, RightsFlags.UpdateCourse)
        //                 && CheckRole(privilegesVal, RightsFlags.GradeAssignment)
        //                 && CheckRole(privilegesVal, RightsFlags.GradeForum)
        //                 && CheckRole(privilegesVal, RightsFlags.GradeExam)
        //                 && CheckRole(privilegesVal, RightsFlags.SetupGradebook)
        //                 && CheckRole(privilegesVal, RightsFlags.ReadGradebook)
        //                 && CheckRole(privilegesVal, RightsFlags.SubmitFinalGrade)
        //                 && CheckRole(privilegesVal, RightsFlags.ReadCourseFull))
        //        {
        //            role = Roles.Teacher;
        //        }
        //        else if (CheckRole(privilegesVal, RightsFlags.ReadCourse)
        //                 && CheckRole(privilegesVal, RightsFlags.UpdateCourse)
        //                 && CheckRole(privilegesVal, RightsFlags.ReadCourseFull))
        //        {
        //            role = Roles.Author;
        //        }
        //        else if (CheckRole(privilegesVal, RightsFlags.Participate)
        //                 && CheckRole(privilegesVal, RightsFlags.ReadCourse))
        //        {
        //            role = Roles.Student;
        //        }
        //        else if (CheckRole(privilegesVal, RightsFlags.ReadCourse))
        //        {
        //            role = Roles.Reader;
        //        }
        //    }

        //    role = Inflector.Capitalize(role);
        //    return role;
        //}

        /////// <summary>
        /////// The format groups xml.
        /////// </summary>
        /////// <param name="session">
        /////// The session.
        /////// </param>
        /////// <param name="courseId">
        /////// The course id.
        /////// </param>
        /////// <returns>
        /////// The <see cref="string"/>.
        /////// </returns>
        ////// ReSharper disable once UnusedMember.Local
        ////private static string FormatGroupsXml(Session session, int courseId)
        ////{
        ////    var xml = "<groups/>";
        ////    var groupsResult = session.Get(Commands.Groups.List, string.Format(Parameters.Groups.List, courseId));
        ////    if (groupsResult != null)
        ////    {
        ////        var groups = groupsResult.XPathSelectElements("/enrollments/enrollment").ToList();
        ////        if (groups.Any())
        ////        {
        ////            xml = "<groups>";
        ////            // ReSharper disable once LoopCanBeConvertedToQuery
        ////            foreach (XElement group in groups)
        ////            {
        ////                string groupId = group.XPathEvaluate("string(@id)").ToString();
        ////                xml += string.Format(@"<group id=""{0}"" />", groupId);
        ////            }

        ////            xml += "</groups>";
        ////        }
        ////    }

        ////    return xml;
        ////}

        ///// <summary>
        ///// The check role.
        ///// </summary>
        ///// <param name="privilegesVal">
        ///// The privileges val.
        ///// </param>
        ///// <param name="roleToCheck">
        ///// The role to check.
        ///// </param>
        ///// <returns>
        ///// The <see cref="bool"/>.
        ///// </returns>
        //private static bool CheckRole(long privilegesVal, RightsFlags roleToCheck)
        //{
        //    return ((RightsFlags)privilegesVal & roleToCheck) == roleToCheck;
        //}

        ///// <summary>
        ///// The add more details for user.
        ///// </summary>
        ///// <param name="api">
        ///// The API.
        ///// </param>
        ///// <param name="usertoken">
        ///// The user token.
        ///// </param>
        ///// <param name="user">
        ///// The user.
        ///// </param>
        //public void AddMoreDetailsForUser(string api, string usertoken, LmsUserDTO user)
        //{
        //    var client = CreateRestClient(api);

        //    RestRequest request = CreateRequest(
        //        api, 
        //        string.Format("/api/v1/users/{0}/profile", user.id), 
        //        Method.GET, 
        //        usertoken);

        //    IRestResponse<LmsUserDTO> response = client.Execute<LmsUserDTO>(request);

        //    if (response.Data != null)
        //    {
        //        user.primary_email = response.Data.primary_email;
        //    }
        //}

        ///// <summary>
        ///// The answer questions for quiz.
        ///// </summary>
        ///// <param name="api">
        ///// The API.
        ///// </param>
        ///// <param name="usertoken">
        ///// The user token.
        ///// </param>
        ///// <param name="submission">
        ///// The submission.
        ///// </param>
        //public void AnswerQuestionsForQuiz(string api, string usertoken, QuizSubmissionDTO submission)
        //{
        //    var client = CreateRestClient(api);

        //    RestRequest request = CreateRequest(
        //        api, 
        //        string.Format("/api/v1/quiz_submissions/{0}/questions", submission.id), 
        //        Method.POST, 
        //        usertoken);
        //    request.RequestFormat = DataFormat.Json;
        //    request.AddBody(submission);

        //    var res = client.Execute(request);
        //}

        ///// <summary>
        ///// The create announcement.
        ///// </summary>
        ///// <param name="api">
        ///// The API.
        ///// </param>
        ///// <param name="usertoken">
        ///// The user token.
        ///// </param>
        ///// <param name="courseid">
        ///// The course id.
        ///// </param>
        ///// <param name="title">
        ///// The title.
        ///// </param>
        ///// <param name="message">
        ///// The message.
        ///// </param>
        ///// <returns>
        ///// The <see cref="AnnouncementDTO"/>.
        ///// </returns>
        //public AnnouncementDTO CreateAnnouncement(
        //    string api, 
        //    string usertoken, 
        //    int courseid, 
        //    string title, 
        //    string message)
        //{
        //    var client = CreateRestClient(api);
        //    RestRequest request = CreateRequest(
        //        api, 
        //        string.Format("/api/v1/courses/{0}/discussion_topics", courseid), 
        //        Method.POST, 
        //        usertoken);
        //    request.AddParameter("title", title);
        //    request.AddParameter("message", message);
        //    request.AddParameter("is_announcement", true);

        //    IRestResponse<AnnouncementDTO> response = client.Execute<AnnouncementDTO>(request);

        //    return response.Data;
        //}

        ///// <summary>
        ///// The get questions for quiz.
        ///// </summary>
        ///// <param name="api">
        ///// The API.
        ///// </param>
        ///// <param name="usertoken">
        ///// The user token.
        ///// </param>
        ///// <param name="courseid">
        ///// The course id.
        ///// </param>
        ///// <param name="quizid">
        ///// The quiz id.
        ///// </param>
        ///// <returns>
        ///// The <see cref="List{QuizQuestionDTO}"/>.
        ///// </returns>
        //public List<QuizQuestionDTO> GetQuestionsForQuiz(string api, string usertoken, int courseid, int quizid)
        //{
        //    var ret = new List<QuizQuestionDTO>();
        //    var client = CreateRestClient(api);

        //    RestRequest request = CreateRequest(
        //        api, 
        //        string.Format("/api/v1/courses/{0}/quizzes/{1}/questions", courseid, quizid), 
        //        Method.GET, 
        //        usertoken);

        //    IRestResponse<List<QuizQuestionDTO>> response = client.Execute<List<QuizQuestionDTO>>(request);

        //    ret.AddRange(response.Data);

        //    return ret;
        //}

        ///// <summary>
        ///// The get course.
        ///// </summary>
        ///// <param name="api">
        ///// The api.
        ///// </param>
        ///// <param name="usertoken">
        ///// The usertoken.
        ///// </param>
        ///// <param name="courseid">
        ///// The courseid.
        ///// </param>
        ///// <returns>
        ///// The <see cref="CourseDTO"/>.
        ///// </returns>
        //public CourseDTO GetCourse(string api, string usertoken, int courseid)
        //{
        //    var client = CreateRestClient(api);

        //    RestRequest request = CreateRequest(
        //        api,
        //        string.Format("/api/v1/courses/{0}", courseid),
        //        Method.GET,
        //        usertoken);

        //    IRestResponse<CourseDTO> response = client.Execute<CourseDTO>(request);

        //    return response.Data;
        //}

        ///// <summary>
        ///// The get quizzes for course.
        ///// </summary>
        ///// <param name="detailed">
        ///// The detailed.
        ///// </param>
        ///// <param name="api">
        ///// The API.
        ///// </param>
        ///// <param name="usertoken">
        ///// The user token.
        ///// </param>
        ///// <param name="courseid">
        ///// The course id.
        ///// </param>
        ///// <returns>
        ///// The <see cref="List{QuizDTO}"/>.
        ///// </returns>
        //public IEnumerable<LmsQuizDTO> GetQuizzesForCourse(bool detailed, string api, string usertoken, int courseid)
        //{
        //    var ret = new List<LmsQuizDTO>();
        //    var client = CreateRestClient(api);

        //    RestRequest request = CreateRequest(
        //        api, 
        //        string.Format("/api/v1/courses/{0}/quizzes", courseid), 
        //        Method.GET, 
        //        usertoken);

        //    IRestResponse<List<LmsQuizDTO>> response = client.Execute<List<LmsQuizDTO>>(request);

        //    if (detailed)
        //    {
        //        foreach (LmsQuizDTO q in response.Data)
        //        {
        //            q.questions = GetQuestionsForQuiz(api, usertoken, courseid, q.id).ToArray();
        //        }
        //    }

        //    ret.AddRange(response.Data);

        //    return ret;
        //}

        ///// <summary>
        ///// The get quiz by id.
        ///// </summary>
        ///// <param name="api">
        ///// The api.
        ///// </param>
        ///// <param name="usertoken">
        ///// The usertoken.
        ///// </param>
        ///// <param name="courseid">
        ///// The courseid.
        ///// </param>
        ///// <param name="quizid">
        ///// The quizid.
        ///// </param>
        ///// <returns>
        ///// The <see cref="LmsQuizDTO"/>.
        ///// </returns>
        //public LmsQuizDTO GetQuizById(string api, string usertoken, int courseid, string quizid)
        //{
        //    var client = CreateRestClient(api);

        //    RestRequest request = CreateRequest(
        //        api,
        //        string.Format("/api/v1/courses/{0}/quizzes/{1}", courseid, quizid),
        //        Method.GET,
        //        usertoken);

        //    IRestResponse<LmsQuizDTO> response = client.Execute<LmsQuizDTO>(request);

        //    response.Data.questions = GetQuestionsForQuiz(api, usertoken, courseid, response.Data.id).ToArray();

        //    return response.Data;
        //}

        ///// <summary>
        ///// The get submission for quiz.
        ///// </summary>
        ///// <param name="api">
        ///// The API.
        ///// </param>
        ///// <param name="usertoken">
        ///// The user token.
        ///// </param>
        ///// <param name="courseid">
        ///// The course id.
        ///// </param>
        ///// <param name="quizid">
        ///// The quiz id.
        ///// </param>
        ///// <returns>
        ///// The <see cref="List{QuizSubmissionDTO}"/>.
        ///// </returns>
        //public List<QuizSubmissionDTO> GetSubmissionForQuiz(
        //    string api, 
        //    string usertoken, 
        //    int courseid, 
        //    int quizid)
        //{
        //    var ret = new List<QuizSubmissionDTO>();
        //    var client = CreateRestClient(api);

        //    RestRequest request = CreateRequest(
        //        api, 
        //        string.Format("/api/v1/courses/{0}/quizzes/{1}/submissions", courseid, quizid), 
        //        Method.POST, 
        //        usertoken);

        //    IRestResponse<QuizSubmissionResultDTO> response = client.Execute<QuizSubmissionResultDTO>(request);

        //    ret.AddRange(response.Data.quiz_submissions);

        //    return ret;
        //}

        

        ///// <summary>
        ///// The return submission for quiz.
        ///// </summary>
        ///// <param name="api">
        ///// The API.
        ///// </param>
        ///// <param name="usertoken">
        ///// The user token.
        ///// </param>
        ///// <param name="courseid">
        ///// The course id.
        ///// </param>
        ///// <param name="submission">
        ///// The submission.
        ///// </param>
        //public static void ReturnSubmissionForQuiz(
        //    string api, 
        //    string usertoken, 
        //    int courseid, 
        //    QuizSubmissionDTO submission)
        //{
        //    var client = CreateRestClient(api);

        //    RestRequest request = CreateRequest(
        //        api,
        //        string.Format("/api/v1/courses/{0}/quizzes/{1}/submissions/{2}/complete", courseid, submission.quiz_id, submission.id),
        //        Method.POST,
        //        usertoken);
        //    request.AddParameter("attempt", submission.attempt);
        //    request.AddParameter("validation_token", submission.validation_token);

        //    client.Execute(request);
        //} */

        ///// <summary>
        ///// The login if necessary.
        ///// </summary>
        ///// <typeparam name="T">
        ///// Any type
        ///// </typeparam>
        ///// <param name="session">
        ///// The session.
        ///// </param>
        ///// <param name="action">
        ///// The action.
        ///// </param>
        ///// <param name="lmsCompany">
        ///// The company LMS.
        ///// </param>
        ///// <param name="error">
        ///// The error.
        ///// </param>
        ///// <returns>
        ///// The <see cref="bool"/>.
        ///// </returns>
        //private T LoginIfNecessary<T>(Session session, Func<Session, T> action, LmsCompany lmsCompany, out string error)
        //{
        //    error = null;
        //    session = session ?? this.BeginBatch(out error, lmsCompany);
        //    if (session != null)
        //    {
        //        return action(session);
        //    }

        //    return default(T);
        //}

        //#endregion

        /// <summary>
        /// The signal types.
        /// </summary>
        public class SignalTypes
        {
            /// <summary>
            /// The enrollment changed.
            /// </summary>
            public const string EnrollmentChanged = "1.2";

            /// <summary>
            /// The course deleted.
            /// </summary>
            public const string CourseDeleted = "4.3";

            /// <summary>
            /// The course created.
            /// </summary>
            public const string CourseCreated = "4.8";
        }

        ///// <summary>
        /////     The commands.
        ///// </summary>
        //protected class Commands
        //{
        //    /// <summary>
        //    ///     The enrollments.
        //    /// </summary>
        //    public class Enrollments
        //    {
        //        #region Constants

        //        /// <summary>
        //        /// The list.
        //        /// </summary>
        //        public const string List = "listenrollments";

        //        /// <summary>
        //        /// The get one.
        //        /// </summary>
        //        public const string GetOne = "getenrollment3";

        //        #endregion
        //    }

        //    /// <summary>
        //    /// The enrollments.
        //    /// </summary>
        //    public class Signals
        //    {
        //        #region Constants

        //        /// <summary>
        //        /// The list.
        //        /// </summary>
        //        public const string List = "getsignallist";

        //        /// <summary>
        //        /// The get last signal id.
        //        /// </summary>
        //        public const string GetLastSignalId = "getlastsignalid";

        //        #endregion
        //    }

        //    /// <summary>
        //    ///     The groups.
        //    /// </summary>
        //    public class Groups
        //    {
        //        #region Constants

        //        /// <summary>
        //        /// The list.
        //        /// </summary>
        //        public const string List = "getgrouplist";

        //        #endregion
        //    }

        //    /// <summary>
        //    ///     The courses.
        //    /// </summary>
        //    public class Courses
        //    {
        //        #region Constants

        //        /// <summary>
        //        /// The get one.
        //        /// </summary>
        //        public const string GetOne = "getcourse2";

        //        #endregion
        //    }

        //    /// <summary>
        //    ///     The announcements.
        //    /// </summary>
        //    public class Announcements
        //    {
        //        #region Constants

        //        /// <summary>
        //        /// The put.
        //        /// </summary>
        //        public const string Put = "putannouncement";

        //        #endregion
        //    }
        //}

        ///// <summary>
        ///// The parameters.
        ///// </summary>
        //protected class Parameters
        //{
        //    /// <summary>
        //    /// The enrollments.
        //    /// </summary>
        //    public class Enrollments
        //    {
        //        #region Constants

        //        /// <summary>
        //        /// The list.
        //        /// </summary>
        //        public const string List = "domainid={0}&limit=0&coursequery=%2Fid%3D{1}&select=user";

        //        /// <summary>
        //        /// The get one.
        //        /// </summary>
        //        public const string GetOne = "enrollmentid={0}&select=user";

        //        #endregion
        //    }

        //    /// <summary>
        //    /// The enrollments.
        //    /// </summary>
        //    public class Signals
        //    {
        //        #region Constants

        //        /// <summary>
        //        /// The list.
        //        /// </summary>
        //        public const string List = "lastsignalid={0}&domainid={1}&type={2}";

        //        #endregion
        //    }

        //    /// <summary>
        //    /// The enrollments.
        //    /// </summary>
        //    public class Courses
        //    {
        //        #region Constants

        //        /// <summary>
        //        /// The get one.
        //        /// </summary>
        //        public const string GetOne = "courseid={0}";

        //        #endregion
        //    }

        //    /// <summary>
        //    /// The groups.
        //    /// </summary>
        //    public class Groups
        //    {
        //         #region Constants

        //        /// <summary>
        //        /// The list.
        //        /// </summary>
        //        public const string List = "ownerid={0}";

        //        #endregion
        //    }

        //    /// <summary>
        //    ///     The announcements.
        //    /// </summary>
        //    public class Announcements
        //    {
        //        #region Constants

        //        /// <summary>
        //        /// The put.
        //        /// </summary>
        //        public const string Put = "entityid={0}&path={1}";

        //        #endregion
        //    }
        //}

        ///// <summary>
        ///// The roles.
        ///// </summary>
        //protected class Roles
        //{
        //    #region Constants

        //    /// <summary>
        //    /// The author.
        //    /// </summary>
        //    public const string Author = "author";

        //    /// <summary>
        //    /// The owner.
        //    /// </summary>
        //    public const string Owner = "owner";

        //    /// <summary>
        //    /// The reader.
        //    /// </summary>
        //    public const string Reader = "reader";

        //    /// <summary>
        //    /// The student.
        //    /// </summary>
        //    public const string Student = "student";

        //    /// <summary>
        //    /// The teacher.
        //    /// </summary>
        //    public const string Teacher = "teacher";

        //    #endregion
        //}
    }
}