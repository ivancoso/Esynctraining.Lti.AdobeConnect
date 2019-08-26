namespace Esynctraining.AC.Provider.Entities
{
    public enum StatusCodes
    {
        not_set,
        /// <summary>
        /// Indicates that the action has completed successfully.
        /// </summary>
        ok,
        /// <summary>
        /// Indicates that a call is invalid in some way. The invalid element provides more detail.
        /// </summary>
        invalid,
        /// <summary>
        /// Indicates that you don’t have permission to call the action. The subcode
        /// attribute provides more details.
        /// </summary>
        no_access,
        /// <summary>
        /// Indicates that there is no data available (in response to an action that
        /// would normally result in returning data). Usually indicates that there is
        /// no item with the ID you specified. To resolve the error, change the
        /// specified ID to that of an item that exists.
        /// </summary>
        no_data,
        /// <summary>
        /// Indicates that the action should have returned a single result but is
        /// actually returning multiple results. For example, if there are multiple
        /// users with the same user name and password, and you call the login
        /// action using that user name and password as parameters, the system
        /// cannot determine which user to log you in as, so it returns a too-muchdata error.
        /// </summary>
        too_much_data,
        internal_error,
        too_much_meetings,
        template_not_found,

        // <status code="operation-size-error" subcode="10000"/>
        // If result count is more than 20000(default value, can be other value)
        operation_size_error,
    }

    public enum StatusSubCodes
    {
        not_set,
        /// <summary>
        /// The customer account has expired.
        /// </summary>
        account_expired,
        /// <summary>
        /// Based on the supplied credentials, you don’t have permission to call the action.
        /// </summary>
        denied,
        /// <summary>
        /// The user is not logged in. To resolve the error, log in (using the login action) before you make the call. For more information, see login.
        /// </summary>
        no_login,
        /// <summary>
        /// The account limits have been reached or exceeded.
        /// </summary>
        no_quota,
        /// <summary>
        /// The required resource is unavailable.
        /// </summary>
        not_available,
        /// <summary>
        /// You must use SSL to call this action.
        /// </summary>
        not_secure,
        /// <summary>
        /// The account is not yet activated.
        /// </summary>
        pending_activation,
        /// <summary>
        /// The account’s license agreement has not been settled.
        /// </summary>
        pending_license,
        /// <summary>
        /// The course or tracking content has expired.
        /// </summary>
        sco_expired,
        /// <summary>
        /// The meeting or course has not started.
        /// </summary>
        sco_not_started,
        //--------------------
        /// <summary>
        /// The call attempted to add a duplicate item in a context where
        /// uniqueness is required.
        /// </summary>
        duplicate,
        /// <summary>
        /// The requested operation violates integrity rules (for example, moving
        /// a folder into itself).
        /// </summary>
        illegal_operation,
        /// <summary>
        /// The requested information does not exist.
        /// </summary>
        no_such_item,
        /// <summary>
        /// The value is outside the permitted range of values.
        /// </summary>
        range,
        /// <summary>
        /// A required parameter is missing.
        /// </summary>
        missing,
        /// <summary>
        /// A passed parameter had the wrong format.
        /// </summary>
        format,
        /// <summary>
        /// Recording is already been converted to MP4.
        /// </summary>
        invalid_recording_job_in_progress,

        /// <summary>
        /// seminar session has already been scheduled for this time
        /// </summary>
        session_schedule_conflict,

        /// <summary>
        /// moving folder into itself
        /// </summary>
        illegal_parent,
        /// <summary>
        /// event limit reached
        /// </summary>
        limit_reached,
        /// <summary>
        /// Login Suspended.
        // To protect your account, your Login has been temporarily suspended for the next 5 minutes due to too many incorrect Login attempts.
        /// </summary>
        user_suspended
    }

    public enum ScoType
    {
        not_set,
        content,
        course,
        curriculum,
        Event,
        folder,
        link,
        meeting,
        session,
        tree,
        external_event,
        seminarsession,
    }

    public enum PrincipalType
    {
        admins,
        authors,
        course_admins,
        event_admins,
        event_group,
        everyone,
        external_group,
        external_user,
        group,
        guest,
        learners,
        live_admins,
        seminar_admins,
        user,

        /// <summary>
        /// Name: Webinar 100 Managers
        /// Description: Users with access to create and run seminars with 100 participants
        /// </summary>
        named_webinar_admins_100,
        event_super_admins,
        admins_limited,
        named_vc_admins, //Virtual Classroom Managers
    }

    public enum SpecialPrincipalId
    {
        public_access
    }

    public enum SpecialPermissionId
    {
        /// <summary>
        /// The meeting is public, and anyone who has the URL for the meeting can enter the room.
        /// </summary>
        view_hidden,

        /// <summary>
        /// The meeting is protected, and only registered users and accepted guests can enter the room.
        /// </summary>
        remove,

        /// <summary>
        /// The meeting is private, and only registered users and participants can enter the room.
        /// </summary>
        denied,
    }

    public enum MeetingPermissionId
    {
        none,
        host,
        mini_host,
        view,
        remove,
        //http://help.adobe.com/en_US/connect/8.0/webservices/WS8d7bb3e8da6fb92f73b3823d121e63182fe-8000_SP1.html#WS5b3ccc516d4fbf351e63e3d11a171ddf77-7fe9_SP1
        //publish,
        //manage,
        //denied
    }

    public enum MeetingFeatureId
    {
        meeting_passcode_notallowed
    }

    public enum AclFieldId
    {
        meeting_passcode,
        telephony_profile,
        seminar_expected_load,
    }

    public enum PermissionId
    {
        none,
        /// <summary>
        /// The principal can view, but cannot modify, the SCO. The principal can take a
        /// course, attend a meeting as participant, or view a folder’s content.
        /// </summary>
        view,
        /// <summary>
        /// Available for meetings only. The principal is host of a meeting and can
        /// create the meeting or act as presenter, even without view permission on the
        /// meeting’s parent folder.
        /// </summary>
        view_hidden,
        /// <summary>
        /// Default for public-access principal permission for content folders
        /// </summary>
        view_only,
        /// <summary>
        /// Not sure what it means and when it's set. Saw this for recording created for user's meeting that had view-hidden permission (Minnesot State)
        /// </summary>
        view_denied,
        /// <summary>
        /// The meeting is public, and anyone who has the URL for the meeting can enter the room.
        /// </summary>
        public_access,
        /// <summary>
        /// Public, equivalent to Anyone who has the URL for the meeting can enter the room.
        /// </summary>
        host,
        /// <summary>
        /// Available for meetings only. The principal is presenter of a meeting and
        /// can present content, share a screen, send text messages, moderate
        /// questions, create text notes, broadcast audio and video, and push content
        /// from web links.
        /// </summary>
        mini_host,
        /// <summary>
        /// Available for meetings only. The principal does not have participant,
        /// presenter or host permission to attend the meeting. If a user is already
        /// attending a live meeting, the user is not removed from the meeting until the
        /// session times out.
        /// </summary>
        remove,
        /// <summary>
        /// Available for SCOs other than meetings. The principal can publish or
        /// update the SCO. The publish permission includes view and allows the
        /// principal to view reports related to the SCO. On a folder, publish does not
        /// allow the principal to create new subfolders or set permissions.
        /// </summary>
        publish,
        /// <summary>
        /// Available for SCOs other than meetings or courses. The principal can
        /// view, delete, move, edit, or set permissions on the SCO. On a folder, the
        /// principal can create subfolders or view reports on folder content.
        /// </summary>
        manage,
        /// <summary>
        /// Available for SCOs other than meetings. The principal cannot view,
        /// access, or manage the SCO.
        /// </summary>
        denied,
        /// <summary>
        /// The meeting is private, and only registered users and participants can enter the room.
        /// </summary>
    }

    // https://helpx.adobe.com/adobe-connect/webservices/common-xml-elements-attributes.html#object_type
    public enum ObjectType
    {
        /// <summary>
        /// An account that contains principals and SCOs.
        /// </summary>
        object_type_account,

        /// <summary>
        /// An action in the Web Services XML API. 
        /// </summary>
        object_type_action,


        /// <summary>
        /// An Adobe Connect event. 
        /// </summary>
        object_type_event,
        
        /// <summary>
        /// A SCO that is not visible in Adobe Connect Central (or in your application, if you use this value).
        /// </summary>
        object_type_hidden,

        /// <summary>
        /// An Adobe Connect meeting.
        /// </summary>
        object_type_meeting,

        /// <summary>
        /// A user or group.
        /// </summary>
        object_type_principal,

        /// <summary>
        /// A setting indicating that Adobe Connect Central displays some data, but a user cannot set the data in Adobe Connect Central.
        /// </summary>
        object_type_read_only,

        /// <summary>
        /// A SCO representing a meeting, course, curriculum, piece of content, folder, or any other object on the server.
        /// </summary>
        object_type_sco,
 
    }

    // https://helpx.adobe.com/adobe-connect/webservices/common-xml-elements-attributes.html#icon
    public enum ScoIcon
    {
        not_set,
        //documented:
        archive,
        attachment,
        authorware,
        captivate,
        course,
        curriculum,
        external_event,
        flv,
        html,
        image,
        lms_plugin,
        logos,
        meeting_template,
        mp3,
        pdf,
        pod,
        presentation,
        producer,
        seminar,
        session,
        swf,
        //not documented:
        content,
        folder,
        pod_hybrid,
        virtual_classroom,
    }

}
