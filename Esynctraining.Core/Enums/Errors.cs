namespace Esynctraining.Core.Enums
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// The errors.
    /// </summary>
    // ReSharper disable InconsistentNaming
    public static class Errors
    {
        /// <summary>
        /// The code error type session expired.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Reviewed. Suppression is OK here.")]
        public const int TOO_MANY_DEPOSITIONS = 508;

        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Reviewed. Suppression is OK here.")]
        public const int TOO_MANY_USERS = 509;

        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Reviewed. Suppression is OK here.")]
        public const int TEMPLATE_NOT_FOUND = 510;

        /// <summary>
        /// The code error type session expired.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Reviewed. Suppression is OK here.")]
        public const int CODE_ERRORTYPE_SESSION_EXPIRED = 101;

        /// <summary>
        /// The code error type user existing.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Reviewed. Suppression is OK here.")]
        public const int CODE_ERRORTYPE_USER_EXISTING = 102;

        /// <summary>
        /// The code error type invalid login.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Reviewed. Suppression is OK here.")]
        public const int CODE_ERRORTYPE_INVALID_LOGIN = 103;

        /// <summary>
        /// The code error type invalid user.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Reviewed. Suppression is OK here.")]
        public const int CODE_ERRORTYPE_INVALID_USER = 104;

        /// <summary>
        /// The code error type user inactive.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Reviewed. Suppression is OK here.")]
        public const int CODE_ERRORTYPE_USER_INACTIVE = 105;

        /// <summary>
        /// The code error type invalid parameter.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Reviewed. Suppression is OK here.")]
        public const int CODE_ERRORTYPE_INVALID_PARAMETER = 106;

        /// <summary>
        /// The code error type object not found.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Reviewed. Suppression is OK here.")]
        public const int CODE_ERRORTYPE_OBJECT_NOT_FOUND = 107;

        /// <summary>
        /// The code error type invalid session.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Reviewed. Suppression is OK here.")]
        public const int CODE_ERRORTYPE_INVALID_SESSION = 108;

        /// <summary>
        /// The code error type invalid object.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Reviewed. Suppression is OK here.")]
        public const int CODE_ERRORTYPE_INVALID_JOB = 110;

        /// <summary>
        /// The code error type invalid object.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Reviewed. Suppression is OK here.")]
        public const int CODE_ERRORTYPE_INVALID_OBJECT = 109;

        /// <summary>
        /// The code error type invalid access.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Reviewed. Suppression is OK here.")]
        public const int CODE_ERRORTYPE_INVALID_ACCESS = 401;

        /// <summary>
        /// The code error type request not processed.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Reviewed. Suppression is OK here.")]
        public const int CODE_ERRORTYPE_REQUEST_NOT_PROCESSED = 501;

        /// <summary>
        /// The code error type generic error.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Reviewed. Suppression is OK here.")]
        public const int CODE_ERRORTYPE_GENERIC_ERROR = 1010;

        /// <summary>
        /// The code result type success.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Reviewed. Suppression is OK here.")]
        public const string CODE_RESULTTYPE_SUCCESS = "SUCCESS";

        /// <summary>
        /// The code result type partial success.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Reviewed. Suppression is OK here.")]
        public const string CODE_RESULTTYPE_PARTIALSUCCESS = "PARTIALSUCCESS";

        /// <summary>
        /// The code result type error.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Reviewed. Suppression is OK here.")]
        public const string CODE_RESULTTYPE_ERROR = "ERROR";
    }
    // ReSharper restore InconsistentNaming
}