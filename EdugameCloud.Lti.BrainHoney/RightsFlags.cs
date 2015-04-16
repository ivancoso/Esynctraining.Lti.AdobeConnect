namespace EdugameCloud.Lti.BrainHoney
{
    using System;

    /// <summary>
    /// The rights flags.
    /// </summary>
    [Flags]
    internal enum RightsFlags : long
    {
        /// <summary>
        /// The none.
        /// </summary>
        None = 0x00, 

        /// <summary>
        /// The participate.
        /// </summary>
        Participate = 0x01, 

        /// <summary>
        /// The create domain.
        /// </summary>
        CreateDomain = 0x10, 

        /// <summary>
        /// The read domain.
        /// </summary>
        ReadDomain = 0x20, 

        /// <summary>
        /// The update domain.
        /// </summary>
        UpdateDomain = 0x40, 

        /// <summary>
        /// The delete domain.
        /// </summary>
        DeleteDomain = 0x80, 

        /// <summary>
        /// The create user.
        /// </summary>
        CreateUser = 0x100, 

        /// <summary>
        /// The read user.
        /// </summary>
        ReadUser = 0x200, 

        /// <summary>
        /// The update user.
        /// </summary>
        UpdateUser = 0x400, 

        /// <summary>
        /// The delete user.
        /// </summary>
        DeleteUser = 0x800, 

        /// <summary>
        /// The create course.
        /// </summary>
        CreateCourse = 0x10000, 

        /// <summary>
        /// The read course.
        /// </summary>
        ReadCourse = 0x20000, 

        /// <summary>
        /// The update course.
        /// </summary>
        UpdateCourse = 0x40000, 

        /// <summary>
        /// The delete course.
        /// </summary>
        DeleteCourse = 0x80000, 

        /// <summary>
        /// The create section.
        /// </summary>
        CreateSection = 0x100000, 

        /// <summary>
        /// The read section.
        /// </summary>
        ReadSection = 0x200000, 

        /// <summary>
        /// The update section.
        /// </summary>
        UpdateSection = 0x400000, 

        /// <summary>
        /// The delete section.
        /// </summary>
        DeleteSection = 0x800000, 

        /// <summary>
        /// The grade assignment.
        /// </summary>
        GradeAssignment = 0x1000000, 

        /// <summary>
        /// The grade forum.
        /// </summary>
        GradeForum = 0x2000000, 

        /// <summary>
        /// The grade exam.
        /// </summary>
        GradeExam = 0x4000000, 

        /// <summary>
        /// The setup grade book.
        /// </summary>
        SetupGradebook = 0x8000000, 

        /// <summary>
        /// The control domain.
        /// </summary>
        ControlDomain = 0x10000000, 

        /// <summary>
        /// The control course.
        /// </summary>
        ControlCourse = 0x20000000, 

        /// <summary>
        /// The control section.
        /// </summary>
        ControlSection = 0x40000000, 

        /// <summary>
        /// The read grade book.
        /// </summary>
        ReadGradebook = 0x80000000, 

        /// <summary>
        /// The report domain.
        /// </summary>
        ReportDomain = 0x100000000, 

        /// <summary>
        /// The report course.
        /// </summary>
        ReportCourse = 0x200000000, 

        /// <summary>
        /// The report section.
        /// </summary>
        ReportSection = 0x400000000, 

        /// <summary>
        /// The post domain announcements.
        /// </summary>
        PostDomainAnnouncements = 0x800000000, 

        /// <summary>
        /// The proxy.
        /// </summary>
        Proxy = 0x1000000000, 

        /// <summary>
        /// The report user.
        /// </summary>
        ReportUser = 0x4000000000, 

        /// <summary>
        /// The submit final grade.
        /// </summary>
        SubmitFinalGrade = 0x8000000000, 

        /// <summary>
        /// The control enrollment.
        /// </summary>
        ControlEnrollment = 0x10000000000, 

        /// <summary>
        /// The read enrollment.
        /// </summary>
        ReadEnrollment = 0x20000000000, 

        /// <summary>
        /// The read course full.
        /// </summary>
        ReadCourseFull = 0x40000000000, 

        /// <summary>
        /// The control user.
        /// </summary>
        ControlUser = 0x80000000000, 

        /// <summary>
        /// The read objective.
        /// </summary>
        ReadObjective = 0x100000000000, 

        /// <summary>
        /// The update objective.
        /// </summary>
        UpdateObjective = 0x200000000000, 

        /// <summary>
        /// The read credits.
        /// </summary>
        ReadCredits = 0x400000000000, 

        /// <summary>
        /// The update credits.
        /// </summary>
        UpdateCredits = 0x800000000000, 

    }

}